
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : Character
{
    [Header("Enemy")]
    public NavMeshAgent agent;

    private Transform player;

    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;
 
    public float timeBetweenAttacks;
    bool alreadyAttacked;

    public float distanceToStartChasingAt;
    public float maximumShootingAngle;

    public GameObject startingGunPrefab;
    private Transform gunAttachPoint;
    private Gun enemyGun;

    [Header("For debugging")]
    [SerializeField]
    private bool isAlreadyChasingPlayer;
    [SerializeField]
    private bool playerNearby;

    private Transform raycastPlayerFrom;

    public override BodySystem bodySystem { get; set; }

    private void Awake()
    {
        player = GameObject.Find("PlayerPrefab").transform;
        agent = GetComponent<NavMeshAgent>();
        isAlreadyChasingPlayer = false;
        raycastPlayerFrom = transform.Find("RaycastPlayerFrom");
        gunAttachPoint = transform.Find("GunAttachPoint");

        enemyGun = Instantiate(startingGunPrefab, gunAttachPoint).GetComponent<Gun>();
        enemyGun.BecomeEquipped();
    }

    private void Update()
    {
        agent.speed = GetCurrentMovementSpeed();

        Vector3 playerPosition = player.transform.position + new Vector3(0, 1, 0);

        // Is player near us? (Through walls etc)
        playerNearby = (raycastPlayerFrom.position - playerPosition).magnitude < distanceToStartChasingAt;
        Vector3 playerDirectionFromEnemy = playerPosition - raycastPlayerFrom.position;

        // Do we have a physical sightline to the player?
        bool isPlayerRaycastable = false;
        RaycastHit hit;
        bool rayHit = Physics.Raycast(raycastPlayerFrom.position, playerDirectionFromEnemy, out hit, distanceToStartChasingAt);
        if (rayHit) {
            isPlayerRaycastable = hit.transform == player.transform;
		}

        bool shouldChasePlayer = (playerNearby && isPlayerRaycastable) || (isAlreadyChasingPlayer && playerNearby);

        if (shouldChasePlayer) {
            agent.SetDestination(player.position);
            isAlreadyChasingPlayer = true;

            // tldr get direction to player and rotate towards it
            Vector3 directionToLook = playerDirectionFromEnemy;
            directionToLook.y = 0; // don't rotate 'vertically'
            Quaternion rotation = Quaternion.LookRotation(directionToLook);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, agent.angularSpeed * Time.deltaTime);

            // tldr calculate angle to player and shoot if he's roughly in front of our vision
            float angleBetweenEnemyForwardAndPlayer = Vector3.Angle(playerDirectionFromEnemy, transform.forward);
            if (isPlayerRaycastable && angleBetweenEnemyForwardAndPlayer < maximumShootingAngle)
            {
                Vector3 gunShootDirection = playerPosition - enemyGun.attackPoint.transform.position;
                enemyGun.Shoot(gunShootDirection, false, bodySystem.GetModifierValue(Modifier.GUNSPREAD));
            }
        } else {
            Patrolling();
            isAlreadyChasingPlayer = false;
		}
    }

    private void Patrolling()
    {
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
            agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        
        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }
    private void SearchWalkPoint()
    {
        
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f))
            walkPointSet = true;
    }

	public override void TakeDamage(float damage)
	{
		base.TakeDamage(damage);
        SoundManager.PlaySound(SoundManager.Sound.enemyhit, transform.position);
    }

	public override void OnDeath()
	{
        bodySystem.GoRagdoll();

        Instantiate(startingGunPrefab, gunAttachPoint.position, Quaternion.identity);
        AmmoPickup ammoBox = Instantiate(Resources.Load("Prefabs/Objects/AmmoCube") as GameObject, gunAttachPoint.position, Quaternion.identity).GetComponent<AmmoPickup>();
        ammoBox.ammoTypeToGive = enemyGun.gunType;
        ammoBox.amountOfAmmo = (int)Random.Range(30f, 50f);

        Destroy(gameObject);
    }
}

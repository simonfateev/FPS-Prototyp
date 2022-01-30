
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : Character
{
    public NavMeshAgent agent;

    public Transform player;

    public float health = 50f;
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

 
    public float timeBetweenAttacks;
    bool alreadyAttacked;

  
    public float sightRange;
    public bool playerInSightRange;

    public override BodySystem bodySystem { get; set; }

    private void Awake()
    {
        player = GameObject.Find("PlayerPrefab").transform;
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange);
       

        if (!playerInSightRange) Patroling();
        if (playerInSightRange) ChasePlayer();
        
    }

    private void Patroling()
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

    private void ChasePlayer()
    {
        agent.SetDestination(player.position);
    }

    public void TakeDamage(float amount)
    {
        SoundManager.PlaySound(SoundManager.Sound.enemyhit, transform.position);
        health -= amount;
        if (health <= 0)
        {
            Die();
        }
        Debug.Log("Health = " + health);
    }

    void Die()
    {
        bodySystem.GoRagdoll();
        Destroy(gameObject);
    }
}

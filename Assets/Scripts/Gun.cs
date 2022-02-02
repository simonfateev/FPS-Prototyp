
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Gun : MonoBehaviour, IInteractable
{
    //Gun stats
    public int damage;
    public float timeBetweenShots, spread, range;
    public int bulletsPerTap;
    public bool allowButtonHold;
    public GunType gunType;

    //bools 
    public bool shooting, readyToShoot, reloading;

    //Reference
    public Transform attackPoint;
    public RaycastHit rayHit;
    private Rigidbody rb;
    public GameObject selfPrefab;
    public string prefabName;
    private AudioSource gunsound;

    //Graphics
    public GameObject muzzleFlash, bulletHoleGraphic;
    public CamShake camShake;
    public float camShakeMagnitude, camShakeDuration;
    public TextMeshProUGUI AmmunitionText;
    public ParticleSystem hitEffect, hitEffectEnemy;
    public TrailRenderer trailEffect;


    private void Awake()
    {
        readyToShoot = true;
        rb = GetComponent<Rigidbody>();
        selfPrefab = Resources.Load(prefabName) as GameObject;
        gunsound = GetComponent<AudioSource>();
    }

    public void SetAttachedToPlayer(PlayerScript playerScript)
    {
        rb.isKinematic = true;
    }

    public void Shoot(Vector3 directionToShoot)
    {
        readyToShoot = false;
        Invoke("ResetShot", timeBetweenShots);

        //Spread
        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);

        //Calculate Direction with Spread
        Vector3 finalDirection = directionToShoot + new Vector3(Random.Range(-spread, spread), Random.Range(-spread, spread), Random.Range(-spread, spread));

        gunsound.Play();

        //RayCast
        if (Physics.Raycast(Camera.main.transform.position, finalDirection, out rayHit, range))
        {
            Character characterHit = rayHit.transform.GetComponent<Character>();
            if (characterHit != null)
            {
                hitEffectEnemy.transform.position = rayHit.point;
                hitEffectEnemy.transform.forward = rayHit.normal;
                hitEffectEnemy.Emit(5);

                characterHit.TakeDamage(damage);
            }
            else
            {
                hitEffect.transform.position = rayHit.point;
                hitEffect.transform.forward = rayHit.normal;
                hitEffect.Emit(5);
            }
        }

        //Graphics
        Instantiate(muzzleFlash, attackPoint);

        var tracer = Instantiate(trailEffect, attackPoint.position, Quaternion.identity);
        tracer.AddPosition(attackPoint.position);

        GameObject newHole = Instantiate(bulletHoleGraphic, rayHit.point + rayHit.normal * 0.00f, Quaternion.identity);
        newHole.transform.LookAt(rayHit.point + rayHit.normal);
        newHole.transform.position += newHole.transform.forward / 1000;
        Destroy(newHole, 15f);

        tracer.transform.position = rayHit.point;
    }

    private void ResetShot()
    {
        readyToShoot = true;
    }

	public void OnInteract(InteractInfo interactInfo)
	{
        interactInfo.byPlayer.EquipGun(this, interactInfo.side);
        Destroy(gameObject);
    }
}

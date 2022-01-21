
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Gun : MonoBehaviour, IPickupableObject
{
    //Gun stats
    public int damage;
    public float timeBetweenShots, spread, range, reloadTime;
    public int magazineSize, bulletsPerTap;
    public bool allowButtonHold;
    public int bulletsLeft;
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

    private void Awake()
    {
        bulletsLeft = magazineSize;
        readyToShoot = true;
        rb = GetComponent<Rigidbody>();
        selfPrefab = Resources.Load(prefabName) as GameObject;
        gunsound = GetComponent<AudioSource>();
    }

    public void SetAttachedToPlayer(PlayerScript playerScript)
    {
        rb.isKinematic = true;
    }

    public void Shoot()
    {
        readyToShoot = false;
        Invoke("ResetShot", timeBetweenShots);

        //Spread
        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);

        //Calculate Direction with Spread
        Vector3 direction = Camera.main.transform.forward + new Vector3(x, y, 0);

        gunsound.Play();

        //RayCast
        if (Physics.Raycast(Camera.main.transform.position, direction, out rayHit, range))
        {
            Debug.Log(rayHit.collider.name);

            if (rayHit.collider.CompareTag("Enemy"))
                rayHit.collider.GetComponent<EnemyAI>().TakeDamage(damage);
        }

        //Graphics
        Instantiate(muzzleFlash, attackPoint.position, Quaternion.identity);
        GameObject newHole = Instantiate(bulletHoleGraphic, rayHit.point + rayHit.normal * 0.00f, Quaternion.identity);
        newHole.transform.LookAt(rayHit.point + rayHit.normal);
        newHole.transform.position += newHole.transform.forward / 1000;
        Destroy(newHole, 15f);
    }

    private void ResetShot()
    {
        readyToShoot = true;
    }

    public void OnPickUp(PlayerScript byPlayer)
    {
        Debug.Log("Gun script got picked up");
    }
}

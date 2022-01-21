
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Gun : MonoBehaviour, IPickupableObject
{
    //Gun stats
    public int damage;
    public float timeBetweenShooting, timeBetweenShots, spread, range, reloadTime;
    public int magazineSize, bulletsPerTap;
    public bool allowButtonHold;
    public int bulletsLeft;
    public int bulletsShot;

    //bools 
    public bool shooting, readyToShoot, reloading;

    //Reference
    public Transform attackPoint;
    public RaycastHit rayHit;
    private Rigidbody rb;
    public GameObject selfPrefab;
    public string prefabName;
    private AudioSource gunsound;
    GameObject player = GameObject.Find("PlayerPrefab");

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
    private void Update()
    {
        //AmmunitionText.SetText(PlayerScript.ammoPistol.ToString());
        //Debug.Log(PlayerScript.ammoPistol.ToString());
        //Debug.Log(gameObject.tag);

        //currentGun = gameObject.tag;

    }
    public void SetAttachedToPlayer(PlayerScript playerScript)
    {
        rb.isKinematic = true;
    }

    public void Shoot()
    {

        readyToShoot = false;

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

        //ShakeCamera
        //camShake.Shake(camShakeDuration, camShakeMagnitude);

        //Graphics
        Instantiate(muzzleFlash, attackPoint.position, Quaternion.identity);
        GameObject newHole = Instantiate(bulletHoleGraphic, rayHit.point + rayHit.normal * 0.00f, Quaternion.identity);
        newHole.transform.LookAt(rayHit.point + rayHit.normal);
        newHole.transform.position += newHole.transform.forward / 1000;
        Destroy(newHole, 15f);

        // Self explanatory but reduces the player ammo reserve depending on weapon type
        switch (gameObject.tag)
        {
            case "pistol":

                PlayerScript.ammoPistol--;

                if (PlayerScript.ammoPistol > 0)
                {
                    bulletsShot--;
                    Invoke("ResetShot", timeBetweenShooting);
                    Invoke("Shoot", timeBetweenShots);
                }

                Debug.Log("pistol ammo -1 " + PlayerScript.ammoPistol.ToString());
                break;

            case "rifle":

                PlayerScript.ammoRifle--;

                if (PlayerScript.ammoRifle > 0)
                {
                    Invoke("ResetShot", timeBetweenShooting);
                    Invoke("Shoot", timeBetweenShots);
                }

                Debug.Log("rifle ammo -1 " + PlayerScript.ammoRifle.ToString());
                break;

                // Using this you can expand as you see fit for different weapons
        }

        Debug.Log(bulletsShot);

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

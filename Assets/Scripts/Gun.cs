using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour, IPickupableObject
{

    public float damage = 10f;
    public float range = 100f;
    public GameObject muzzleflash;
    public GameObject bulletHoleGraphic;
    public bool isFiring;

    public GameObject selfPrefab;
    public string prefabName;

    private Camera cam;
    private AudioSource gunsound;
    private Rigidbody rb;

    void Awake()
    {
        isFiring = false;
        cam = Camera.main;
        muzzleflash.SetActive(false);
        gunsound = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody>();
        selfPrefab = Resources.Load(prefabName) as GameObject;
    }

    void Update() 
    {
        if (isFiring) {
            muzzleflash.SetActive(true);  
        } else {
            muzzleflash.SetActive(false); 
        }
    }

    public void SetAttachedToPlayer(PlayerScript playerScript) {
        rb.isKinematic = true;
	}

    public void Shoot()
    {
        gunsound.Play(); 

        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, range))
        {
            Enemy enemy = hit.transform.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }

        GameObject newHole = Instantiate(bulletHoleGraphic, hit.point + hit.normal * 0.00f, Quaternion.identity);
        newHole.transform.LookAt(hit.point + hit.normal);
        newHole.transform.position += newHole.transform.forward / 1000;
        Destroy(newHole, 15f);

    }

	public void OnPickUp(PlayerScript byPlayer)
	{
        Debug.Log("Gun script got picked up");
	}
}

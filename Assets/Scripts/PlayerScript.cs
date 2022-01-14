using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    private float range = 10f;
    
    public Gun currentRightGun;
    public Gun currentLeftGun;
    public Camera cam;
    

    void Start()
    {
        
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            AttemptPickup();
        }

        if (Input.GetButtonDown("Fire1"))
        {
            currentRightGun.isFiring = true;
            currentRightGun.Shoot();
        }

        if (Input.GetButtonUp("Fire1"))
        {
            currentRightGun.isFiring = false;
        }

        if (Input.GetButtonDown("Fire2"))
        {
            currentLeftGun.isFiring = true;
            currentLeftGun.Shoot();
        }

        if (Input.GetButtonUp("Fire2"))
        {
            currentLeftGun.isFiring = false;
        }
    }

    void AttemptPickup()
    {
        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, range))
        {
            Debug.Log(hit.transform.name);

            PickupableObject obj = hit.transform.GetComponent<PickupableObject>();
            if (obj != null)
            {
                obj.OnPickup();
            }
        }

    }    
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    private enum Side {
        LEFT,
        RIGHT
	}

    private float range = 10f;
    
    public GameObject startingLeftGunPrefab;
    public GameObject startingRightGunPrefab;

    public Transform gunAttachPointLeft;
    public Transform gunAttachPointRight;
    public Transform gunDropPoint;
    public Gun gun;
    public bool shooting;

    public Camera cam;

    private Dictionary<Side, Gun> playerGuns = new Dictionary<Side, Gun>();
    private Dictionary<Side, Transform> attachPoints = new Dictionary<Side, Transform>();

    void Start()
    {
        // Setup dictionaries
        playerGuns.Add(Side.LEFT, null);
        playerGuns.Add(Side.RIGHT, null);
        attachPoints.Add(Side.LEFT, gunAttachPointLeft);
        attachPoints.Add(Side.RIGHT, gunAttachPointRight);

        EquipGun(startingLeftGunPrefab.GetComponent<Gun>(), Side.LEFT);
        EquipGun(startingRightGunPrefab.GetComponent<Gun>(), Side.RIGHT);
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            AttemptPickup();
        }

        MyInput();
    }

    public void MyInput()
    {
        // Right side gun input code this was ass to write
        if (playerGuns[Side.RIGHT].allowButtonHold) shooting = Input.GetKey(KeyCode.Mouse0);
        else shooting = Input.GetKeyDown(KeyCode.Mouse0);
        if (playerGuns[Side.RIGHT].readyToShoot && shooting && !playerGuns[Side.RIGHT].reloading && playerGuns[Side.RIGHT].bulletsLeft > 0)
        {
            playerGuns[Side.RIGHT].bulletsShot = playerGuns[Side.RIGHT].bulletsPerTap;
            playerGuns[Side.RIGHT].Shoot();
        }

        // Left side gun input code this was equally bad
        if (playerGuns[Side.LEFT].allowButtonHold) shooting = Input.GetKey(KeyCode.Mouse1);
        else shooting = Input.GetKeyDown(KeyCode.Mouse1);
        if (playerGuns[Side.LEFT].readyToShoot && shooting && !playerGuns[Side.LEFT].reloading && playerGuns[Side.LEFT].bulletsLeft > 0)
        {
            playerGuns[Side.LEFT].bulletsShot = playerGuns[Side.LEFT].bulletsPerTap;
            playerGuns[Side.LEFT].Shoot();
        }

        // Reload input code
        if (Input.GetKeyDown(KeyCode.R))
        {
            playerGuns[Side.LEFT].Reload();
            playerGuns[Side.RIGHT].Reload();
        }
    }
    void AttemptPickup()
    {
        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, range))
        {
            Debug.Log(hit.transform.name);

            IPickupableObject obj = hit.transform.GetComponent<IPickupableObject>();
            if (obj != null)
            {
                // Tell the pickupable that this specific player got it
                // no use right now but I figure you'll need it
                obj.OnPickUp(this);

                Gun gunScript = hit.transform.GetComponent<Gun>();
				if (gunScript)
				{
                    EquipGun(gunScript, Side.RIGHT);
                    Destroy(hit.transform.gameObject);
				}
            }
        }

    }

    void DropGun(Side side) {
        if (playerGuns[side] != null) {
            // Wipe everything in the current attach point
            foreach (Transform child in attachPoints[side])
            {
				Destroy(child.gameObject);
            }

            // Drop a prefab in front of the player
            Gun gunToDrop = playerGuns[side];
            Instantiate(gunToDrop.selfPrefab, gunDropPoint.position, Quaternion.identity);
            playerGuns[side] = null;
        }
    }

    void EquipGun(Gun gun, Side side) {
        DropGun(side);

        GameObject newGun = Instantiate(gun.selfPrefab, attachPoints[side], false);
        Gun gunScript = newGun.GetComponent<Gun>();

        gunScript.SetAttachedToPlayer(this);
        playerGuns[side] = gunScript;
    }
}

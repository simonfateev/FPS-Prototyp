using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour
{
    private enum Side
    {
        LEFT,
        RIGHT
    }

    private float range = 10f;

    public GameObject startingLeftGunPrefab;
    public GameObject startingRightGunPrefab;

    public Transform gunAttachPointLeft;
    public Transform gunAttachPointRight;
    public Transform gunDropPoint;
    public bool shooting;

    public Camera cam;

    private Dictionary<Side, Gun> playerGuns = new Dictionary<Side, Gun>();
    private Dictionary<Side, Transform> attachPoints = new Dictionary<Side, Transform>();

    // player held ammo values
    public static int ammoPistol;
    public static int ammoRifle;

    public TextMeshProUGUI ammoDisplayLeft;
    public TextMeshProUGUI ammoDisplayRight;

    void Start()
    {
        // Setup dictionaries
        playerGuns.Add(Side.LEFT, null);
        playerGuns.Add(Side.RIGHT, null);
        attachPoints.Add(Side.LEFT, gunAttachPointLeft);
        attachPoints.Add(Side.RIGHT, gunAttachPointRight);

        EquipGun(startingLeftGunPrefab.GetComponent<Gun>(), Side.LEFT);
        EquipGun(startingRightGunPrefab.GetComponent<Gun>(), Side.RIGHT);

        ammoPistol = 10;
        ammoRifle = 30;
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            AttemptPickup();
        }

        MyInput();
        AmmoDisplay();
    }

    private KeyCode getKeyForGunSide(Side side)
    {
        if (side == Side.LEFT)
        {
            return KeyCode.Mouse1;
        }
        else
        {
            return KeyCode.Mouse0;
        }
    }

    public void MyInput()
    {
        foreach (Side side in Enum.GetValues(typeof(Side)))
        {

            if (playerGuns[side].allowButtonHold)
                shooting = Input.GetKey(getKeyForGunSide(side));
            else
                shooting = Input.GetKeyDown(getKeyForGunSide(side));

            switch (playerGuns[side].tag)
            {
                case "pistol":

                    if (playerGuns[side].readyToShoot && shooting && ammoPistol > 0)
                    {
                        playerGuns[side].Shoot();
                    }

                    break;

                case "rifle":

                    if (playerGuns[side].readyToShoot && shooting && ammoRifle > 0)
                    {
                        playerGuns[side].Shoot();
                    }

                    break;
            }
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

    void DropGun(Side side)
    {
        if (playerGuns[side] != null)
        {
            // Wipe everything in the current attach point
            foreach (Transform child in attachPoints[side])
            {
                Destroy(child.gameObject);
            }

            // Drop a prefab in front of the player
            Gun gunToDrop = playerGuns[side];
            GameObject newGunObj = Instantiate(gunToDrop.selfPrefab, gunDropPoint.position, Quaternion.identity);
            Gun newGunScript = newGunObj.GetComponent<Gun>();

            newGunScript.bulletsLeft = playerGuns[side].bulletsLeft;
            playerGuns[side] = null;
        }
    }

    void EquipGun(Gun gunScriptToEquip, Side side)
    {
        DropGun(side);

        GameObject newGun = Instantiate(gunScriptToEquip.selfPrefab, attachPoints[side], false);
        Gun newGunScript = newGun.GetComponent<Gun>();

        newGunScript.SetAttachedToPlayer(this);
        newGunScript.bulletsLeft = gunScriptToEquip.bulletsLeft;
        playerGuns[side] = newGunScript;
    }


    void AmmoDisplay()
    {
        switch (playerGuns[Side.LEFT].tag)
        {
            case "pistol":
                ammoDisplayLeft.SetText(ammoPistol.ToString());
                break;

            case "rifle":
                ammoDisplayLeft.SetText(ammoRifle.ToString());
                break;
        }

        switch (playerGuns[Side.RIGHT].tag)
        {
            case "pistol":
                ammoDisplayRight.SetText(ammoPistol.ToString());
                break;

            case "rifle":
                ammoDisplayRight.SetText(ammoRifle.ToString());
                break;
        }
    }
}

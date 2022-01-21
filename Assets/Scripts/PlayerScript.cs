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

    public Transform gunAttachPointLeft;
    public Transform gunAttachPointRight;
    public Transform gunDropPoint;
    public bool shooting;

    public Camera cam;

    private Dictionary<Side, Gun> playerGuns = new Dictionary<Side, Gun>();
    private Dictionary<Side, Transform> attachPoints = new Dictionary<Side, Transform>();

    private Dictionary<GunType, int> ammoStorage = new Dictionary<GunType, int>();

    public TextMeshProUGUI ammoDisplayLeft;
    public TextMeshProUGUI ammoDisplayRight;
    private Dictionary<Side, TextMeshProUGUI> ammoDisplays = new Dictionary<Side, TextMeshProUGUI>();

    void Start()
    {
        // Setup vars
        playerGuns.Add(Side.LEFT, null);
        playerGuns.Add(Side.RIGHT, null);
        attachPoints.Add(Side.LEFT, gunAttachPointLeft);
        attachPoints.Add(Side.RIGHT, gunAttachPointRight);

        ammoStorage[GunType.PISTOL] = 7;
        ammoStorage[GunType.RIFLE] = 30;
        ammoStorage[GunType.SHOTGUN] = 8;
        ammoStorage[GunType.SNIPER] = 10;

        ammoDisplays.Add(Side.LEFT, ammoDisplayLeft);
        ammoDisplays.Add(Side.RIGHT, ammoDisplayRight);

        // Function setup?
        UpdateAmmoDisplays();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            AttemptPickup(Side.RIGHT);
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            AttemptPickup(Side.LEFT);
        }

        MyInput();

        UpdateAmmoDisplays();
    }

    public void AddAmmo(GunType gunType, int change) {
        ammoStorage[gunType] += change;
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

    void MyInput()
    {
        foreach (Side side in Enum.GetValues(typeof(Side)))
        {
            Gun gun = playerGuns[side];
            if (gun == null)
                continue; // if no gun on this side skip this side

            // if allowButtonHold true, use GetKey, otherwise GetKeyDown
            shooting = gun.allowButtonHold ? Input.GetKey(getKeyForGunSide(side)) : Input.GetKeyDown(getKeyForGunSide(side));

            if (shooting) {
                bool hasAmmo = ammoStorage[gun.gunType] > 0;

                if (gun.readyToShoot && hasAmmo) {
                    gun.Shoot();

                    ammoStorage[gun.gunType]--;
                }
			}
        }
    }

    void AttemptPickup(Side side)
    {
        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, range))
        {
            Debug.Log(hit.transform.name);

            Gun gunScript = hit.transform.GetComponent<Gun>();
            if (gunScript)
            {
                EquipGun(gunScript, side);
                Destroy(hit.transform.gameObject);
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

            playerGuns[side] = null;
        }
    }

    void EquipGun(Gun gunScriptToEquip, Side side)
    {
        DropGun(side);

        GameObject newGun = Instantiate(gunScriptToEquip.selfPrefab, attachPoints[side], false);
        Gun newGunScript = newGun.GetComponent<Gun>();

        newGunScript.SetAttachedToPlayer(this);
        playerGuns[side] = newGunScript;
    }

    void UpdateAmmoDisplays()
    {
        // For each side, set the text of the relevant ammo display to the remaning ammo of that gun type
        foreach (Side side in Enum.GetValues(typeof(Side))) {
            string ammoText = playerGuns[side] ? ammoStorage[playerGuns[side].gunType].ToString() : " ";
            ammoDisplays[side].SetText(ammoText);
		}
    }

    void OnControllerColliderHit(ControllerColliderHit hit) {
        IPickupable pickupable = hit.gameObject.GetComponent<IPickupable>();
        if (pickupable != null) {
            pickupable.OnPickUp(this);
		}
	}
}

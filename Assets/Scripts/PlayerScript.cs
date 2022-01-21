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

        EquipGun(startingLeftGunPrefab.GetComponent<Gun>(), Side.LEFT);
        EquipGun(startingRightGunPrefab.GetComponent<Gun>(), Side.RIGHT);

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
        if (Input.GetKeyDown(KeyCode.F))
        {
            AttemptPickup();
        }

        MyInput();
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
            Gun gun = playerGuns[side];

            // if allowButtonHold true, use GetKey, otherwise GetKeyDown
            shooting = gun.allowButtonHold ? Input.GetKey(getKeyForGunSide(side)) : Input.GetKeyDown(getKeyForGunSide(side));

            if (shooting) {
                bool hasAmmo = ammoStorage[gun.gunType] > 0;

                if (gun.readyToShoot && hasAmmo) {
                    gun.Shoot();

                    ammoStorage[gun.gunType]--;
                    UpdateAmmoDisplays();
                }
			}
        }
    }

    void AttemptPickup()
    {
        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, range))
        {
            Debug.Log(hit.transform.name);

            Gun gunScript = hit.transform.GetComponent<Gun>();
            if (gunScript)
            {
                EquipGun(gunScript, Side.RIGHT);
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

    void UpdateAmmoDisplays()
    {
        // For each side, set the text of the relevant ammo display to the remaning ammo of that gun type
        foreach (Side side in Enum.GetValues(typeof(Side))) {
            ammoDisplays[side].SetText(ammoStorage[playerGuns[side].gunType].ToString());
		}
    }
}

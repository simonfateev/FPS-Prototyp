using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerScript : Character
{
    public enum Side
    {
        LEFT,
        RIGHT
    }

    public static PlayerScript player;

    private float range = 10f;

    [Header("PlayerScript")]
    public Transform gunAttachPointLeft;
    public Transform gunAttachPointRight;
    public Transform gunDropPoint;
    public bool shooting;

    public Camera cam;

    private Dictionary<Side, Gun> playerGuns = new Dictionary<Side, Gun>();
    private Dictionary<Side, Transform> attachPoints = new Dictionary<Side, Transform>();

    private Dictionary<GunType, int> ammoStorage = new Dictionary<GunType, int>();

    public override BodySystem bodySystem { get; set; }

    public GameObject deathScreen;

    public GameObject onHitUI;
    Animator hitEffectAnim;

	void Awake()
	{
        // Scuffed singleton
        player = this;
        SoundManager.Initialize();

        Time.timeScale = 1.0f;
        deathScreen.SetActive(false);

        hitEffectAnim = onHitUI.GetComponent<Animator>();
    }

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
        ammoStorage[GunType.SMG] = 32;

        deathScreen.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            AttemptPickup(Side.RIGHT);
        }

        if (Input.GetKeyDown(KeyCode.Q) && bodySystem.BodyHasPassive(Passives.DUALWIELD))
        {
            AttemptPickup(Side.LEFT);
        }

        MyInput();
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

                if (hasAmmo) {
                    bool didGunShoot = gun.Shoot(Camera.main.transform.forward, false, bodySystem.GetModifierValue(Modifier.GUNSPREAD));

                    if (didGunShoot) {
                        ammoStorage[gun.gunType]--;
                    }
                }
                else
                {
                    SoundManager.PlaySound(SoundManager.Sound.noammo, transform.position);
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

            IInteractable interactable = hit.transform.GetComponent<IInteractable>();
            if (interactable != null)
            {
                interactable.OnInteract(new InteractInfo(this, side));
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

    public void EquipGun(Gun gunScriptToEquip, Side side)
    {
        DropGun(side);

        GameObject newGun = Instantiate(gunScriptToEquip.selfPrefab, attachPoints[side], false);
        Gun newGunScript = newGun.GetComponent<Gun>();

        newGunScript.BecomeEquipped();
        playerGuns[side] = newGunScript;
    }

    // Get gun for side, null if doesn't exist
    public Gun GetGunForSide(Side side) {
        return playerGuns[side] ? playerGuns[side] : null;
	}

    public int GetAmmoForGun(GunType gt) {
        return ammoStorage[gt];
	}

    void OnControllerColliderHit(ControllerColliderHit hit) {
        IPickupable pickupable = hit.gameObject.GetComponent<IPickupable>();
        if (pickupable != null) {
            pickupable.OnPickUp(this);
		}
	}

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        SoundManager.PlaySound(SoundManager.Sound.enemyhit, transform.position);
        hitEffectAnim.Play("onHit");
    }

    public override void OnDeath()
    {
        Debug.Log("player ded");
        deathScreen.SetActive(true);
        Time.timeScale = 0.0f;
	}

	public override void OnBodyPartSwap()
	{
		base.OnBodyPartSwap();
        if (playerGuns[Side.LEFT] != null && !bodySystem.BodyHasPassive(Passives.DUALWIELD)) {
            DropGun(Side.LEFT);
		}
	}
}

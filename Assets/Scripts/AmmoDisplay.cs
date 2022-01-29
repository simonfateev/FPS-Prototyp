using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AmmoDisplay : MonoBehaviour
{
    public PlayerScript.Side side;
    public TextMeshProUGUI ammoText;
    public RawImage ammoIcon;
	public GameObject ammoPane;

	private static Dictionary<GunType, Texture> ammoIcons;

	void Awake()
	{
		// This is probably one of the worst ways to do this
		if (ammoIcons == null) {
			ammoIcons = new Dictionary<GunType, Texture>();
			foreach (GunType gt in Enum.GetValues(typeof(GunType)))
			{
				Texture ammoIcon = Resources.Load("Textures/AmmoIcons/ammo_" + gt.ToString()) as Texture;
				ammoIcons.Add(gt, ammoIcon);
			}
		}
	}

	void Update()
	{
		// Also not the best way
		Gun gun = PlayerScript.player.GetGunForSide(side);
		if (gun != null) {
			ammoPane.SetActive(true);
			ammoText.text = PlayerScript.player.GetAmmoForGun(gun.gunType).ToString();
			ammoIcon.texture = ammoIcons[gun.gunType];
		} else {
			ammoPane.SetActive(false);
		}
	}
}

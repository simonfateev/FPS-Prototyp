using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class AmmoPickup : MonoBehaviour, IPickupable
{
    [SerializeField] private GunType ammoTypeToGive;
    [SerializeField] private int amountOfAmmo;

    private bool hasBeenPickedUp = false;

	public void OnPickUp(PlayerScript byPlayer)
	{
        if (!hasBeenPickedUp) {
            byPlayer.AddAmmo(ammoTypeToGive, amountOfAmmo);

            // Put sounds, particles here etc whatever you want when it's picked up

            Destroy(gameObject);
        }
    }
}

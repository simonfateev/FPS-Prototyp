using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPickupableObject
{
    void OnPickUp(PlayerScript byPlayer);
}

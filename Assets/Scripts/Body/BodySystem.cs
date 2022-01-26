using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodySystem : MonoBehaviour
{
    // BodyPartType
    // DONE - enum

    // BodySystem
    // DONE - PlayerScript reference both ways
    // DONE - stores one BodyPart for each BodyPartType
    // DONE - stat methods go through each BodyPart and collect stats
    // DONE - each part gets special ability (or just body, doesn't matter)
    // Specific key targets specific BodyPartType special
    // DONE - Basic head model with RigidBody, colliders
    // DONE - Basic torso model with RigidBody, colliders
    // DONE - Basic legs model with RigidBody, colliders
    // Activating ragdoll body parts
    // DONE - Attaching body parts
    // Pickupable by player

    // BodyPart
    // DONE - list of Modifier
    // DONE - optional special ability field of type SpecialAbility

    // Modifier
    // DONE - enum

    // SpecialAbility interface
    // DONE - one method that gets the PlayerScript passed

    [Header("References")]
    public PlayerScript attachedPlayer;

    [Header("Starting parts")]
    public GameObject startingHead;
    public GameObject startingTorso;
    public GameObject startingLegs;
    private Dictionary<BodyPartType, BodyPart> bodyParts = new Dictionary<BodyPartType, BodyPart>();

	private void Start()
	{
        EquipBodyPart(startingHead);
        EquipBodyPart(startingTorso);
        EquipBodyPart(startingLegs);
    }

    private void EquipBodyPart(GameObject bodyPartPrefab) {
        BodyPart bp = bodyPartPrefab.GetComponent<BodyPart>();
        BodyPartType type = bp.type;
        Transform attachPoint = transform.Find(type.ToString()); // finds 'HEAD', 'TORSO' object underneath this gameobject

        // Wipe everything in the current attach point
        foreach (Transform child in attachPoint)
        {
            Destroy(child.gameObject);
        }

        GameObject newBpObj = Instantiate(bodyPartPrefab, attachPoint, false);
		BodyPart newBp = newBpObj.GetComponent<BodyPart>();
        newBp.rb.isKinematic = true;
        bodyParts[type] = newBp;
    }

    public float getModifierValue(Modifier mod) {
        // Go through all body parts, if they have the modifier we're looking for, collect it
        // finally sum them all up and return
        float total = 0f;
        foreach (BodyPartType bpt in Enum.GetValues(typeof(BodyPartType))) {
            if (bodyParts[bpt].modifiers.ContainsKey(mod)) {
                total += bodyParts[bpt].modifiers[mod];
            }
		}
        return total;
    }

    public void useAbilityOn(BodyPartType bpt) {
        BodyPart bp = bodyParts[bpt];
        if (bp.specialAbility != null) {
            bp.specialAbility.UseAbility(attachedPlayer);
		}
	}
}

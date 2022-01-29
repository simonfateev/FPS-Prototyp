using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodySystem : MonoBehaviour
{
    [Header("References")]
    public Character attachedToChar;

    [Header("Starting parts")]
    public GameObject startingHead;
    public GameObject startingTorso;
    public GameObject startingLegs;
    private Dictionary<BodyPartType, BodyPart> bodyParts = new Dictionary<BodyPartType, BodyPart>();

    private Dictionary<BodyPartType, Transform> bodyAttachPoints = new Dictionary<BodyPartType, Transform>();

	private void Start()
	{
        // finds 'HEAD', 'TORSO', 'LEGS' object underneath this gameobject
        foreach (BodyPartType bpt in Enum.GetValues(typeof(BodyPartType)))
        {
            bodyAttachPoints.Add(bpt, transform.Find(bpt.ToString()));
        }

        attachedToChar = GetComponent<Character>();
        if (attachedToChar != null) attachedToChar.bodySystem = this;

        EquipBodyPart(startingHead);
        EquipBodyPart(startingTorso);
        EquipBodyPart(startingLegs);
    }

    private void EquipBodyPart(GameObject bodyPartPrefab) {
        BodyPart bp = bodyPartPrefab.GetComponent<BodyPart>();
        BodyPartType type = bp.type;

        // Wipe everything in the current attach point
        foreach (Transform child in bodyAttachPoints[type])
        {
            Destroy(child.gameObject);
        }

        GameObject newBpObj = Instantiate(bodyPartPrefab, bodyAttachPoints[type], false);
		BodyPart newBp = newBpObj.GetComponent<BodyPart>();
        newBp.rb.isKinematic = true;
        bodyParts[type] = newBp;
        newBp.parentSystem = this;
    }

    public void SwapBodyPart(BodyPart bpToEquip) {
        // Drop current part
        BodyPart bpToDrop = bodyParts[bpToEquip.type];
        GameObject newDroppedBp = Instantiate(bpToDrop.getSelfPrefab(), transform.position + (transform.forward), Quaternion.identity); // spawn new body part in front of player

        // Equip part
        EquipBodyPart(bpToEquip.getSelfPrefab());
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
            if (GetComponent<PlayerScript>() != null) bp.specialAbility.UseAbility(GetComponent<PlayerScript>());
        }
	}

    // This will not destroy the existing body parts, they are assumed to be deleted soon anyway if this 'body' has died
    public void goRagdoll() {
        foreach (BodyPartType bpt in Enum.GetValues(typeof(BodyPartType))) {
            BodyPart currentBP = bodyParts[bpt];
            Instantiate(currentBP.getSelfPrefab(), bodyAttachPoints[bpt].position, Quaternion.identity);
		}
    }
}

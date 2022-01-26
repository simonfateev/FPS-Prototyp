using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPart : MonoBehaviour
{
	// Scuffed fake dictionary because Unity is scuffed
	[System.Serializable]
	struct Stat {
		public Modifier modifier;
		public float value;
	}

	[SerializeField]
	private List<Stat> statList = new List<Stat>(); // only for starting, don't use directly
	public Dictionary<Modifier, float> modifiers = new Dictionary<Modifier, float>();

	public BodyPartType type;
	public ISpecialAbility specialAbility;

	public Rigidbody rb;

	private void Awake()
	{
		rb = GetComponent<Rigidbody>();

		// Turn fake dict into a real dict
		foreach (Stat stat in statList) {
			if (modifiers.ContainsKey(stat.modifier)) Debug.LogError("YOU DUPLICATED THE MODIFIRES AGHGHGHSGHSD");

			modifiers.Add(stat.modifier, stat.value);
		}
	}
}

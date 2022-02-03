using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPart : MonoBehaviour, IInteractable
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
	public BodySystem parentSystem;
	public string selfPrefabPath;

	private void Awake()
	{
		rb = GetComponent<Rigidbody>();
		specialAbility = GetComponent<ISpecialAbility>();

		// Turn fake dict into a real dict
		foreach (Stat stat in statList) {
			if (modifiers.ContainsKey(stat.modifier)) Debug.LogError("YOU DUPLICATED THE MODIFIRES AGHGHGHSGHSD");

			modifiers.Add(stat.modifier, stat.value);
		}
	}

	public GameObject GetSelfPrefab() {
		return Resources.Load(selfPrefabPath) as GameObject;
	}

	public void OnInteract(InteractInfo interactInfo)
	{
		if (parentSystem == null) {
			BodySystem toAttachTo = interactInfo.byPlayer.bodySystem;
			toAttachTo.SwapBodyPart(this);
			Destroy(gameObject);
		}
	}
}

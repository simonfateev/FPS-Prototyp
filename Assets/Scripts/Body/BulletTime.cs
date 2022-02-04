using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletTime : MonoBehaviour, ISpecialAbility
{
	public float cooldown = 10f;
	private float lastUsed = 0f;

	public void UseAbility(PlayerScript player)
	{
		// activate bullet time if cooldown has passed (check lastUsed against currnet time)
	}

	private void Update()
	{
		// check if bullet time should still be going
		// if not return time to normal
	}

	private void ContinueBullet() {
		Time.timeScale = 0.5f;
	}
}

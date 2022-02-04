using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletTime : MonoBehaviour, ISpecialAbility
{
	public float cooldown;
	public float duration;

	private bool abilityActive;
	private bool cdActive;


    public void UseAbility(PlayerScript player)
	{
		if (!abilityActive && !cdActive)
		{
			StartCoroutine("Duration");
			StartCoroutine("Cooldown");
		}
	}

	public IEnumerator Cooldown()
    {
		cdActive = true;
		yield return new WaitForSeconds(cooldown);
		cdActive = false;
		Debug.Log("ability active now");
    }

	public IEnumerator Duration()
    {
		abilityActive = true;
		Time.timeScale = 0.5f;
		yield return new WaitForSeconds(duration);
		abilityActive = false;
		Time.timeScale = 1.0f;
		Debug.Log("duration expired");
    }
}

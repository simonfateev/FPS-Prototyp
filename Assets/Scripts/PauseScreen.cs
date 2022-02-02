using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class PauseScreen : MonoBehaviour
{
	GameObject[] pauseObjects;

	void Start()
	{
		Time.timeScale = 1;
		pauseObjects = GameObject.FindGameObjectsWithTag("ShowOnPause");
		hidePaused();
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.P))
		{
			if (Time.timeScale == 1.0f)
			{
				Time.timeScale = 0.0f;
				showPaused();
			}
			else if (Time.timeScale == 0.0f)
			{
				Debug.Log("high");
				Time.timeScale = 1.0f;
				hidePaused();
			}
		}
	}

	public void pauseControl()
	{
		if (Time.timeScale == 1.0f)
		{
			Time.timeScale = 0.0f;
			showPaused();
		}
		else if (Time.timeScale == 0.0f)
		{
			Time.timeScale = 1.0f;
			hidePaused();
		}
	}

	public void showPaused()
	{
		foreach (GameObject g in pauseObjects)
		{
			g.SetActive(true);
		}
	}

	public void hidePaused()
	{
		foreach (GameObject g in pauseObjects)
		{
			g.SetActive(false);
		}
	}

}

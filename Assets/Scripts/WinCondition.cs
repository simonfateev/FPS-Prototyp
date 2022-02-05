using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WinCondition : MonoBehaviour
{
    public GameObject winConditionUI;
    public GameObject music;
    public TextMeshProUGUI timerText;
    private int enemiesLeft = 0;
    private int minutes;
    private int seconds;
    private float timeTaken;
    private AudioSource winSound;

    private void Awake()
    {
        winConditionUI.SetActive(false);
        timerText.GetComponent<TextMeshPro>();
        winSound = GetComponent<AudioSource>(); 
    }

    private void Update()
    {
        timeTaken = Time.time;
    }

    public void enemyHasDied()
    {
        enemiesLeft--;
        print(enemiesLeft);

        if (enemiesLeft == 0)
        {
            TimeCalc();
            timerText.SetText("You took: " + minutes + "m " + seconds + "s");
            Destroy(music);
            winConditionUI.SetActive(true);
            winSound.Play();
            Time.timeScale = 0f;
            Debug.Log("Fuck you!");
        }
    }

    public void enemyHasAppeared()
    {
        enemiesLeft++;
        print(enemiesLeft);
    }

    public void TimeCalc()
    {
        timeTaken = Time.time;
        minutes = Mathf.RoundToInt(timeTaken / 60);
        seconds = Mathf.Abs(Mathf.RoundToInt(timeTaken - (minutes * 60)));
    }
}
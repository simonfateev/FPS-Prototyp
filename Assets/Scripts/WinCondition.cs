using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WinCondition : MonoBehaviour
{
    public GameObject winConditionUI;
    public TextMeshProUGUI timerText;
    private int enemiesLeft = 0;
    private int minutes;
    private int seconds;
    private float timeTaken;

    private void Awake()
    {
        winConditionUI.SetActive(false);
        timerText.GetComponent<TextMeshPro>();
    }

    private void Update()
    {
        timeTaken = Time.time;
        Debug.Log(timeTaken);
    }

    public void enemyHasDied()
    {
        enemiesLeft--;
        print(enemiesLeft);

        if (enemiesLeft < 0)
        {
            TimeCalc();
            timerText.SetText("You took: " + minutes + "m " + seconds + "s");
            winConditionUI.SetActive(true);
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
        minutes = Mathf.RoundToInt(timeTaken / 60);
        seconds = Mathf.RoundToInt(timeTaken - (minutes * 60));
        SoundManager.PlaySound(SoundManager.Sound.gamewin, transform.position);
    }
}
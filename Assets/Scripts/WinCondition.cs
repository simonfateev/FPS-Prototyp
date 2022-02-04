using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinCondition : MonoBehaviour
{

    private int enemiesLeft = 0;

    public void enemyHasDied()
    {
        enemiesLeft--;
        print(enemiesLeft);

        if (enemiesLeft == 0)
        {
            Debug.Log("Fuck you!");
        }
    }

    public void enemyHasAppeared()
    {
        enemiesLeft++;
        print(enemiesLeft);
    }
}
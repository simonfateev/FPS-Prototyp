using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    public SimpleHealthBar healthBar;
    Character playerCharacter;
    private void Start()
    {
        playerCharacter = PlayerScript.player.bodySystem.attachedToChar;
    }
    public void Update()
    {
        healthBar.UpdateBar(playerCharacter.GetHealth(), playerCharacter.GetCurrentMaxHealth());
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
    public abstract BodySystem bodySystem { get; set; }

    [Header("General Character")]
    public float defaultMaxHealth;
    public float defaultMovementSpeed;

    [SerializeField] [Tooltip("For debugging, don't change this value")]
    private float currentHealth;

	//
	// Adding a method to character here allows it to be used on both enemies and the player
	//

	private void Start()
	{
        currentHealth = GetCurrentMaxHealth();
	}

	public float GetCurrentMaxHealth() {
        return defaultMaxHealth + bodySystem.GetModifierValue(Modifier.HEALTH);
	}

    public float GetHealth() {
        return currentHealth;
	}

    public void SetHealth(float health) {
        currentHealth = health;
	}

    public void AddHealth(float health) {
        currentHealth = Mathf.Min(currentHealth + health, GetCurrentMaxHealth());

        Debug.Log(gameObject.ToString() + " healed " + health);
    }

    // BodySystem calls this when you swap a body part to validate current health
    public void RecalculateCurrentHealth() {
        if (currentHealth > GetCurrentMaxHealth()) {
            currentHealth = GetCurrentMaxHealth();
		}
	}

    public float GetCurrentMovementSpeed() {
        return defaultMovementSpeed + bodySystem.GetModifierValue(Modifier.MOVEMENTSPEED);
	}

    public virtual void TakeDamage(float damage) {
        currentHealth -= damage;
        if (currentHealth < 0) {
            OnDeath();
		}

        Debug.Log(gameObject.ToString() + " took " + damage + " damage");
	}

    public abstract void OnDeath();
}

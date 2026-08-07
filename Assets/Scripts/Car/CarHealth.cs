using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarHealth : MonoBehaviour
{
    public HealthBar HealthBar;
    public bool shieldActive = false;
    [Header("Health")]
    public int maxHealth = 100;
    public float currentHealth;

    public bool IsDead => currentHealth <= 0;
    void Update()
    {
    }

    private void Awake()
    {
        currentHealth = maxHealth;
        HealthBar.SetMaxHealth((int)maxHealth);
    }

    public void TakeDamage(float damage)
    {
        if (IsDead)
            return;

        if (shieldActive)
            return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        Debug.Log($"Health : {currentHealth}");
        HealthBar.SetHealth(currentHealth);
        if (currentHealth <= 0)
            Die();
    }

    public void Heal(float amount)
    {
        if (IsDead)
            return;

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        Debug.Log($"Health : {currentHealth}");
        HealthBar.SetHealth(currentHealth);
    }

    private void Die()
    {
        Debug.Log("Vehicle Destroyed");

        // We'll add explosion, respawn and UI later.
    }
}



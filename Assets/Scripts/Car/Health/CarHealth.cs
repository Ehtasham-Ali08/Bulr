using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarHealth : MonoBehaviour
{
    public HealthBar HealthBar;

    [Header("Shield")]
    public bool shieldActive = false;

    [Header("Health")]
    public int maxHealth = 100;
    public float currentHealth;
    private bool raceFinished = false;

    [Header("Death & Respawn")]
    public RCC_CarControllerV4 carController;
    public float respawnDelay = 3f;
    public float respawnBehindDistance = 2.5f;
    public float invincibilityDuration = 2f;
    public float deathStopTime = 0.2f;

    private Vector3 respawnPosition;
    private Quaternion respawnRotation;

    private bool isRespawning = false;
    private bool isInvincible = false;

    public bool IsDead => currentHealth <= 0;

    private void Awake()
    {
        currentHealth = maxHealth;

        HealthBar.SetMaxHealth(maxHealth);
        HealthBar.SetHealth(currentHealth);
    }

    public void TakeDamage(float damage)
    {
        // Don't take damage while dead or respawning
        if (IsDead || isRespawning)
            return;

        if (raceFinished)
            return;

        // Don't take damage while invincible
        if (isInvincible)
            return;

        // Shield blocks damage
        if (shieldActive)
            return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        Debug.Log($"Health : {currentHealth}");

        HealthBar.SetHealth(currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        // Don't heal while dead or respawning
        if (IsDead || isRespawning)
            return;

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        Debug.Log($"Health : {currentHealth}");

        HealthBar.SetHealth(currentHealth);
    }

    public void SetRaceFinished()
    {
        raceFinished = true;
    }

    private void Die()
    {
        if (isRespawning)
            return;

        Debug.Log("Vehicle Destroyed");

        isRespawning = true;

        // Save death position
        Vector3 forward = transform.forward;
        forward.y = 0f;
        forward.Normalize();

        respawnPosition =
            transform.position - forward * respawnBehindDistance;

        respawnRotation = Quaternion.Euler(
      0f,
      transform.eulerAngles.y,
      0f
  );

        // Disable RCC driving
        if (carController != null)
        {
            carController.enabled = false;
        }

        // Immediately stop all movement
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.drag = 0.4f;
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        StartCoroutine(RespawnCar());
    }

    private IEnumerator RespawnCar()
    {
        yield return new WaitForSeconds(respawnDelay);
        Rigidbody rb = GetComponent<Rigidbody>();

        rb.drag = 0.1f;
        // Respawn
        transform.position = respawnPosition;
        transform.rotation = respawnRotation;


        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // Restore health
        currentHealth = maxHealth;
        HealthBar.SetHealth(currentHealth);

        // Reset shield
        shieldActive = false;

        // Enable RCC
        if (carController != null)
        {
            carController.enabled = true;
        }

        isRespawning = false;

        StartCoroutine(InvincibilityTimer());

        Debug.Log("Vehicle Respawned");
    }

    private IEnumerator InvincibilityTimer()
    {
        isInvincible = true;

        Debug.Log("Vehicle is INVINCIBLE");

        yield return new WaitForSeconds(invincibilityDuration);

        isInvincible = false;

        Debug.Log("Vehicle is no longer invincible");
    }
    private IEnumerator StopCar()
    {
        Rigidbody rb = GetComponent<Rigidbody>();

        if (rb == null)
            yield break;

        float elapsed = 0f;

        Vector3 startingVelocity = rb.velocity;
        Vector3 startingAngularVelocity = rb.angularVelocity;

        while (elapsed < deathStopTime)
        {
            float t = elapsed / deathStopTime;

            // Smoothly reduce velocity to zero
            rb.velocity = Vector3.Lerp(startingVelocity, Vector3.zero, t);
            rb.angularVelocity = Vector3.Lerp(
                startingAngularVelocity,
                Vector3.zero,
                t
            );

            elapsed += Time.fixedDeltaTime;

            yield return new WaitForFixedUpdate();
        }

        // Make absolutely sure the car has stopped
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }
}
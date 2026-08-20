using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepairPowerUp : MonoBehaviour
{
    private CarHealth health;

    [Header("Repair Settings")]
    public float healAmount = 40f;

    [Header("Healing Visual")]
    public ParticleSystem healingEffectPrefab;

    private ParticleSystem healingEffect;

    private void Start()
    {
        health = GetComponent<CarHealth>();
    }

    public void Activate()
    {
        // Heal the car
        health.Heal(healAmount);
        Debug.Log("Repair Used");

        // Play healing effect
        PlayHealingEffect();
    }

    private void PlayHealingEffect()
    {
        if (healingEffectPrefab == null)
        {
            Debug.LogWarning("Healing Effect Prefab is not assigned.");
            return;
        }

        // Prevent duplicate effects
        if (healingEffect != null)
        {
            Destroy(healingEffect.gameObject);
            healingEffect = null;
        }

        // Spawn the effect on the car
        healingEffect = Instantiate(
            healingEffectPrefab,
            transform.position,
            Quaternion.identity,
            transform
        );

        // Position it relative to the car
        healingEffect.transform.localPosition = Vector3.zero;

        // Keep the effect upright
        healingEffect.transform.localRotation = Quaternion.identity;

        // Play it and any child particle systems
        healingEffect.Play(true);
    }
}


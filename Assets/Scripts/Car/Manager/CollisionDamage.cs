using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CarHealth))]
public class CollisionDamage : MonoBehaviour
{
    private CarHealth health;

    [Header("Impact Settings")]
    public float lowImpact = 15f;
    public float mediumImpact = 25f;
    public float highImpact = 40f;
    public float maxDamage = 25f;

    [Header("Cooldown")]
    public float hitCooldown = 0.5f;

    private float lastHitTime;

    private void Start()
    {
        health = GetComponent<CarHealth>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (Time.time - lastHitTime < hitCooldown)
            return;
        // --- NEW: LANDING FILTER ---
        // 1. Get the exact world position where the car hit the terrain
        Vector3 contactPoint = collision.contacts[0].point;

        // 2. Convert that point into the car's local coordinate system
        Vector3 localContactPoint = transform.InverseTransformPoint(contactPoint);

        // 3. Define your safety zone height relative to the car's pivot.
        // Adjust this value (-0.5f) based on where your car frame sits.
        float bottomThreshold = -0.5f;

        // 4. If the hit is below this height, it's a bottom landing—ignore it completely!
        if (localContactPoint.y < bottomThreshold)
        {
            return;
        }
        // ----------------------------
        lastHitTime = Time.time;

        float impact = collision.relativeVelocity.magnitude;

        float damage = 0f;

        if (impact >= highImpact)
        {
            damage = maxDamage;
        }
        else if (impact >= mediumImpact)
        {
            damage = maxDamage * 0.5f;
        }
        else if (impact >= lowImpact)
        {
            damage = maxDamage * 0.2f;
        }
        else
        {
            return;
        }

        health.TakeDamage(damage);

        Debug.Log($"Hit {collision.gameObject.name} | Impact: {impact:F1} | Damage: {damage:F1} | RemaningHealth: {health.currentHealth}");
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shockwave : MonoBehaviour
{
    public GameObject owner;

    [Header("Shockwave Assign Area")]
    public ParticleSystem shockwaveEffectPrefab;

    [Header("Shockwave Settings")]
    public float radius = 12f;
    public float damage = 20f;
    public float explosionForce = 2200f;
    public float upwardsModifier = 1f;

    [Header("Visual Settings")]
    public float visualYOffset = -1f;

    private void Start()
    {
        // --------------------------------------------------
        // PLAY SHOCKWAVE VISUAL ON THE OWNER CAR
        // --------------------------------------------------

        if (shockwaveEffectPrefab != null && owner != null)
        {
            ParticleSystem effect = Instantiate(
                shockwaveEffectPrefab,
                owner.transform.position,
                Quaternion.identity,
                owner.transform
            );

            // Position relative to the car
            effect.transform.localPosition =
                new Vector3(0f, visualYOffset, 0f);

            // Keep the shockwave horizontal
            effect.transform.localRotation =
                Quaternion.identity;

            // Play the particle and any child particles
            effect.Play(true);
        }

        // --------------------------------------------------
        // FIND CARS IN RANGE
        // --------------------------------------------------

        Collider[] hits = Physics.OverlapSphere(
            transform.position,
            radius
        );

        // Prevent processing the same car multiple times
        HashSet<CarHealth> processedCars =
            new HashSet<CarHealth>();

        foreach (Collider hit in hits)
        {
            CarHealth health =
                hit.GetComponentInParent<CarHealth>();

            if (health == null)
                continue;

            // Ignore the car that used the shockwave
            if (health.gameObject == owner)
                continue;

            // Skip duplicate colliders belonging to the same car
            if (processedCars.Contains(health))
                continue;

            processedCars.Add(health);

            // --------------------------------------------------
            // DAMAGE
            // --------------------------------------------------

            health.TakeDamage(damage);

            // --------------------------------------------------
            // EXPLOSION FORCE
            // --------------------------------------------------

            Rigidbody rb =
                health.GetComponent<Rigidbody>();

            if (rb != null)
            {
                Debug.Log("RB = " + rb);

                rb.AddExplosionForce(
                    explosionForce,
                    transform.position,
                    radius,
                    upwardsModifier,
                    ForceMode.Impulse
                );
            }
        }

        // Destroy the gameplay Shockwave object.
        // The particle is now a child of the owner car,
        // so it will NOT be destroyed with this object.
        Destroy(gameObject);
    }
}
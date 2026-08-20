using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine : MonoBehaviour
{
    [Header("Explosion Effect")]
    public ParticleSystem blastEffectPrefab;

    public float armTime = 0.1f;
    public float damage = 20f;
    private bool armed = false;

    private void Start()
    {
        StartCoroutine(ArmMine());
    }

    IEnumerator ArmMine()
    {
        yield return new WaitForSeconds(armTime);

        armed = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!armed)
            return;

        CarHealth health = other.GetComponentInParent<CarHealth>();

        if (blastEffectPrefab != null)
        {
            ParticleSystem effect = Instantiate(
                blastEffectPrefab,
                transform.position,
                Quaternion.identity
            );

            effect.Play(true);

            Destroy(effect.gameObject, 2f);
        }
        if (health != null)
        {
            health.TakeDamage(damage);

            Debug.Log("Mine exploded!");

            Destroy(gameObject);
        }

    }
}
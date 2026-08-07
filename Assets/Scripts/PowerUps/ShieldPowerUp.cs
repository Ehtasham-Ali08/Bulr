using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldPowerUp : MonoBehaviour
{
    public float shieldDuration = 5f;

    private CarHealth health;

    private void Start()
    {
        health = GetComponent<CarHealth>();
    }

    public void Activate()
    {
        StartCoroutine(ShieldRoutine());
    }

    private IEnumerator ShieldRoutine()
    {
        health.shieldActive = true;

        Debug.Log("Shield Activated");

        yield return new WaitForSeconds(shieldDuration);

        health.shieldActive = false;

        Debug.Log("Shield Expired");
    }
}
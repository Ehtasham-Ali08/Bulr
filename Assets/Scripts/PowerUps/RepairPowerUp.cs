using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepairPowerUp : MonoBehaviour
{
    private CarHealth health;

    [Header("Repair Settings")]
    public float healAmount = 40f;

    void Start()
    {
        health = GetComponent<CarHealth>();
    }

    public void Activate()
    {
        health.Heal(healAmount);

        Debug.Log("Repair Used");
    }
}


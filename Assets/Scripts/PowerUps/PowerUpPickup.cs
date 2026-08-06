using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PowerUpType;

public class PowerUpPickup : MonoBehaviour
{
    public PowerUpTypes powerUpType;

    private void OnTriggerEnter(Collider other)
    {
        CarPowerUpManager manager =
            other.GetComponentInParent<CarPowerUpManager>();

        if (manager == null)
            return;

        if (manager.HasPowerUp())
            return;

        manager.GivePowerUp(powerUpType);

        Destroy(gameObject);
    }
}

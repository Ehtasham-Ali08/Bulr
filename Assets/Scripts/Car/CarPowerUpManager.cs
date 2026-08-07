using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PowerUpType;

public class CarPowerUpManager : MonoBehaviour
{
    public PowerUpTypes currentPowerUp = PowerUpTypes.None;

    public TurboPowerUp turbo;
    public RepairPowerUp repair;
    public ShieldPowerUp shield;

    public bool HasPowerUp()
    {
        return currentPowerUp != PowerUpTypes.None;
    }

    public void GivePowerUp(PowerUpTypes type)
    {
        currentPowerUp = type;
    }

    public void UsePowerUp()
    {
        switch (currentPowerUp)
        {
            case PowerUpTypes.Turbo:
                turbo.Activate();
                break;

            case PowerUpTypes.Repair:
                repair.Activate();
                break;

            case PowerUpTypes.Shield:
                shield.Activate();
                break;

        }

        currentPowerUp = PowerUpTypes.None;
    }
   
}

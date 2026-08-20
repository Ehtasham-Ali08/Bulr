using UnityEngine;

public class LapFinishTrigger : MonoBehaviour
{
    public RaceManager raceManager;

    [Header("Trigger Type")]
    public bool isCheckpoint = false;

    private void OnTriggerEnter(Collider other)
    {
        RCC_CarControllerV4 car =
            other.GetComponentInParent<RCC_CarControllerV4>();

        if (car == null)
            return;

        if (isCheckpoint)
        {
            raceManager.CarReachedCheckpoint(car);
        }
        else
        {
            raceManager.CarCrossedFinish(car);
        }
    }
}

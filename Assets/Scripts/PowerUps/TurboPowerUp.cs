using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurboPowerUp : MonoBehaviour
{
    private RCC_CarControllerV4 car;
    private Rigidbody rb;

    [Header("Turbo Settings")]
    public float turboDuration = 1.5f;

    [Tooltip("Acceleration force applied during turbo")]
    public float turboForce = 35f;

    public bool turboActive = false;

    void Start()
    {
        car = GetComponent<RCC_CarControllerV4>();
        rb = GetComponent<Rigidbody>();
    }

    public void Activate()
    {
        if (turboActive)
            return;

        StartCoroutine(TurboRoutine());
    }

    IEnumerator TurboRoutine()
    {
        turboActive = true;

        Debug.Log("Turbo Started");

        float timer = 0f;

        while (timer < turboDuration)
        {
            rb.AddForce(car.transform.forward * turboForce, ForceMode.Acceleration);

            timer += Time.deltaTime;
            yield return null;
        }

        Debug.Log("Turbo Ended");

        turboActive = false;
    }
}
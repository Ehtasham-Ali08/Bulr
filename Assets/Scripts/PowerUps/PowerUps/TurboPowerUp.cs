using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering; // Required for Post Processing Volume control

public class TurboPowerUp : MonoBehaviour
{
    private RCC_CarControllerV4 car;
    private Rigidbody rb;

    [Header("Turbo Settings")]
    public float turboDuration = 1.5f;

    [Tooltip("Acceleration force applied during turbo")]
    public float turboForce = 35f;

    public bool turboActive = false;

    [Header("Visual Effects & Camera Setup")]
    [Tooltip("The camera assigned to this player (Main Camera or Player Camera)")]
    public Camera playerCamera;

    [Tooltip("Target FOV during turbo")]
    public float turboFOV = 75f;
    private float defaultFOV = 60f;

    [Tooltip("Speed lines particle system attached to the camera or kart")]
    public ParticleSystem speedLinesParticles;

    [Tooltip("Wheel or exhaust trail renderers")]
    public TrailRenderer[] turboTrails;

    [Tooltip("Optional: URP/HDRP Post-Processing Volume for Motion Blur")]
    public Volume turboPostProcessingVolume;

    void Start()
    {
        car = GetComponent<RCC_CarControllerV4>();
        rb = GetComponent<Rigidbody>();

        // Find camera automatically if not manually assigned
        if (playerCamera == null)
            playerCamera = Camera.main;

        if (playerCamera != null)
            defaultFOV = playerCamera.fieldOfView;

        // Ensure trails are off at start
        SetTrailsEmitting(false);

        // Ensure post processing weight starts at 0
        if (turboPostProcessingVolume != null)
            turboPostProcessingVolume.weight = 0f;
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

        // --- 1. ENABLE EFFECTS ---
        SetTrailsEmitting(true);

        if (speedLinesParticles != null)
            speedLinesParticles.Play();

        float timer = 0f;

        while (timer < turboDuration)
        {
            // Apply Turbo physics
            rb.AddForce(car.transform.forward * turboForce, ForceMode.Acceleration);

            // Lerp Camera FOV outward (Smooth stretch)
            if (playerCamera != null)
            {
                playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, turboFOV, Time.deltaTime * 10f);
            }

            // Lerp Post Processing Motion Blur Weight to 1
            if (turboPostProcessingVolume != null)
            {
                turboPostProcessingVolume.weight = Mathf.Lerp(turboPostProcessingVolume.weight, 1f, Time.deltaTime * 10f);
            }

            timer += Time.deltaTime;
            yield return null;
        }

        Debug.Log("Turbo Ended");

        // --- 2. DISABLE EFFECTS & RESTORE CAMERA ---
        SetTrailsEmitting(false);

        if (speedLinesParticles != null)
            speedLinesParticles.Stop();

        // Smoothly restore Camera FOV and disable Post Processing
        float returnTimer = 0f;
        float returnDuration = 0.3f;
        float currentFOV = playerCamera != null ? playerCamera.fieldOfView : defaultFOV;
        float currentVolumeWeight = turboPostProcessingVolume != null ? turboPostProcessingVolume.weight : 0f;

        while (returnTimer < returnDuration)
        {
            if (playerCamera != null)
            {
                playerCamera.fieldOfView = Mathf.Lerp(currentFOV, defaultFOV, returnTimer / returnDuration);
            }

            if (turboPostProcessingVolume != null)
            {
                turboPostProcessingVolume.weight = Mathf.Lerp(currentVolumeWeight, 0f, returnTimer / returnDuration);
            }

            returnTimer += Time.deltaTime;
            yield return null;
        }

        // Final snap to defaults
        if (playerCamera != null)
            playerCamera.fieldOfView = defaultFOV;

        if (turboPostProcessingVolume != null)
            turboPostProcessingVolume.weight = 0f;

        turboActive = false;
    }

    private void SetTrailsEmitting(bool emit)
    {
        if (turboTrails == null) return;

        foreach (TrailRenderer trail in turboTrails)
        {
            if (trail != null)
                trail.emitting = emit;
        }
    }
}
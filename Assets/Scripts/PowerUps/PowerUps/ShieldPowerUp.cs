using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldPowerUp : MonoBehaviour
{
    public float shieldDuration = 5f;

    private CarHealth health;

    [Header("Shield Visual Setup")]
    [Tooltip("The Sphere Mesh Object containing your Hexagon Shield Material")]
    public GameObject shieldVisual;

    [Tooltip("Child particle system emitting floating hex spangles inside the bubble")]
    public ParticleSystem hexSpanglesParticles;

    [Tooltip("Optional ground ring particle system triggered once when shield activates")]
    public ParticleSystem activationShockwave;

    [Header("Animation Settings")]
    [Tooltip("Target scale for the shield bubble mesh when fully popped open")]
    public Vector3 targetShieldScale = new Vector3(2.5f, 2.5f, 2.5f);

    [Tooltip("How fast the shield pops open and shrinks down")]
    public float scaleAnimationSpeed = 0.15f;

    [Header("Texture Spinning Settings")]
    [Tooltip("Enable texture spinning/scrolling effect")]
    public bool spinTexture = true;

    [Tooltip("Horizontal texture spin speed")]
    public float spinSpeedX = 0.3f;

    [Tooltip("Vertical texture spin speed")]
    public float spinSpeedY = 0.15f;

    private Coroutine activeShieldCoroutine;
    private Material shieldMaterial;
    private Vector2 textureOffset = Vector2.zero;

    private void Start()
    {
        health = GetComponent<CarHealth>();

        // Cache the material from the Shield Visual Mesh Renderer
        if (shieldVisual != null)
        {
            Renderer rend = shieldVisual.GetComponent<Renderer>();
            if (rend != null)
            {
                shieldMaterial = rend.material;
            }

            // Ensure shield visual is hidden and zeroed at start
            shieldVisual.transform.localScale = Vector3.zero;
            shieldVisual.SetActive(false);
        }

        // Ensure ambient particles are stopped at start
        if (hexSpanglesParticles != null)
        {
            hexSpanglesParticles.Stop();
        }
    }

    private void Update()
    {
        // Continuously spin the hexagon texture while shield is active
        if (health != null && health.shieldActive && spinTexture && shieldMaterial != null)
        {
            textureOffset.x += spinSpeedX * Time.deltaTime;
            textureOffset.y += spinSpeedY * Time.deltaTime;

            // Works for Standard Shader (mainTextureOffset)
            shieldMaterial.mainTextureOffset = textureOffset;

            // Fallback for custom shader property names if mainTextureOffset isn't linked
            if (shieldMaterial.HasProperty("_BaseMap_ST"))
            {
                Vector4 tilingOffset = shieldMaterial.GetVector("_BaseMap_ST");
                tilingOffset.z = textureOffset.x;
                tilingOffset.w = textureOffset.y;
                shieldMaterial.SetVector("_BaseMap_ST", tilingOffset);
            }
        }
    }

    public void Activate()
    {
        if (activeShieldCoroutine != null)
        {
            StopCoroutine(activeShieldCoroutine);
        }

        activeShieldCoroutine = StartCoroutine(ShieldRoutine());
    }

    private IEnumerator ShieldRoutine()
    {
        health.shieldActive = true;
        Debug.Log("Shield Activated");

        // 1. Play activation shockwave ground ring
        if (activationShockwave != null)
        {
            activationShockwave.Play();
        }

        // 2. Enable mesh and animate pop-in scale (0 to target scale)
        if (shieldVisual != null)
        {
            shieldVisual.SetActive(true);
            yield return StartCoroutine(AnimateShieldScale(Vector3.zero, targetShieldScale, scaleAnimationSpeed));
        }

        // 3. Start floating internal hex particles
        if (hexSpanglesParticles != null)
        {
            hexSpanglesParticles.Play();
        }

        // Wait out the shield duration
        yield return new WaitForSeconds(shieldDuration);

        // 4. Deactivate effects and animate pop-out scale
        health.shieldActive = false;

        if (hexSpanglesParticles != null)
        {
            hexSpanglesParticles.Stop();
        }

        if (shieldVisual != null)
        {
            yield return StartCoroutine(AnimateShieldScale(targetShieldScale, Vector3.zero, scaleAnimationSpeed));
            shieldVisual.SetActive(false);
        }

        Debug.Log("Shield Expired");
    }

    private IEnumerator AnimateShieldScale(Vector3 startScale, Vector3 endScale, float duration)
    {
        float timer = 0f;

        while (timer < duration)
        {
            shieldVisual.transform.localScale = Vector3.Lerp(startScale, endScale, timer / duration);
            timer += Time.deltaTime;
            yield return null;
        }

        shieldVisual.transform.localScale = endScale;
    }
}
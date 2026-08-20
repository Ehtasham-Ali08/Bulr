using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockwavePowerUp : MonoBehaviour
{
    public GameObject shockwavePrefab;
    public Transform spawnPoint;

    public void Activate()
    {
        GameObject shockwave = Instantiate(
            shockwavePrefab,
            spawnPoint.position,
            Quaternion.identity);

        Shockwave shockwaveScript = shockwave.GetComponent<Shockwave>();

        if (shockwaveScript != null)
        {
            shockwaveScript.owner = gameObject;
        }
    }
}
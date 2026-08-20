using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinePowerUp : MonoBehaviour
{
    public GameObject minePrefab;
    public Transform mineDropPoint;

    public void Activate()
    {
        Instantiate(
        minePrefab,
        mineDropPoint.position + Vector3.up * 0.2f,
        mineDropPoint.rotation
        );
    }
}
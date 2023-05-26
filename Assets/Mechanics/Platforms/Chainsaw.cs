using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chainsaw : MonoBehaviour
{
    [SerializeField] private LineRenderer chains;
    [SerializeField] private GameObject chainSaw;
    
    [SerializeField] private float rotationTime = 2.0f;
    [Range(0f, 90f)][SerializeField] private float maxRotationDegree = 90f;
    
    
    private void Start()
    {
        StartCoroutine(Swing());
    }
    
    private void Update()
    {
        UpdateChains();
    }

    private void UpdateChains()
    {
        var points = new Vector3[2];
        points[0] = transform.position;
        points[1] = chainSaw.transform.position;
        chains.SetPositions(points);
    }

    IEnumerator Swing()
    {
        Quaternion rotationA = Quaternion.Euler(0, 0, maxRotationDegree);  // Rotation A (90 degrees)
        Quaternion rotationB = Quaternion.Euler(0, 0, -maxRotationDegree); // Rotation B (-90 degrees)

        // We start from rotation A
        Quaternion startRotation = rotationA; 
        Quaternion endRotation = rotationB; 

        while (true)
        {
            float elapsedTime = 0.0f;

            while (elapsedTime < rotationTime)
            {
                // We're applying a sinusoidal easing here (slow-fast-slow)
                float t = 0.5f * (1 - Mathf.Cos(Mathf.PI * elapsedTime / rotationTime));
                transform.rotation = Quaternion.Slerp(startRotation, endRotation, t);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Ensure the rotation is exactly at the end rotation when the transition time is over
            transform.rotation = endRotation;

            // Swap the start and end rotations for the next iteration
            (startRotation, endRotation) = (endRotation, startRotation);
        }
    }
}

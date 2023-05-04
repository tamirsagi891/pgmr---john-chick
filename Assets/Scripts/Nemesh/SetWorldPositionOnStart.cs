using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetWorldPositionOnStart : MonoBehaviour
{
    [SerializeField]
    private Vector3 worldPosition = Vector3.zero;

    [SerializeField]
    private bool doThisEffect;

    private void Start()
    {
        if (doThisEffect)
        {
            transform.position = worldPosition;
        }
    }

}

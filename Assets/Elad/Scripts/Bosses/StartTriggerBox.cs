using System;
using System.Collections;
using System.Collections.Generic;
using Elad.Events;
using UnityEngine;

public class StartTriggerBox : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(TagStrings.playerTag))
        {
            BossEvents.StartRoamingFromRunning.Invoke();
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using Elad.Events;
using UnityEngine;

public class StartTriggerBox : MonoBehaviour
{
    enum TriggerBoxKind
    {
        BossStart,
        BossStartRoaming
    }

    [SerializeField] private TriggerBoxKind triggerBoxKind;


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(TagStrings.playerTag))
        {
            switch (triggerBoxKind)
            {
                case TriggerBoxKind.BossStart:
                    StartBoss();
                    break;

                case TriggerBoxKind.BossStartRoaming:
                    StartRoaming();
                    break;
            }
            
            Destroy(gameObject);
        }
    }

    private void StartRoaming()
    {
        BossEvents.StartRoamingFromRunning.Invoke();
        
    }

    private void StartBoss()
    {
        BossEvents.BossStart.Invoke();
    }
}
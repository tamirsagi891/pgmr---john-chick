using System;
using System.Collections;
using System.Collections.Generic;
using Elad.Events;
using UnityEngine;

public class StartTriggerBox : MonoBehaviour
{
    enum TriggerBoxKind
    {
        StopMovement,
        BossStart,
        BossStartRoaming
    }

    [SerializeField] private TriggerBoxKind triggerBoxKind;
    private bool _canWork = true;


    private void OnEnable()
    {
        characterEvents.PlayerDied.AddListener(PlayerDied);
    }

    private void OnDisable()
    {
        characterEvents.PlayerDied.RemoveListener(PlayerDied);
    }
    
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(TagStrings.playerTag) && _canWork)
        {
            _canWork = false;
            switch (triggerBoxKind)
            {
                case TriggerBoxKind.BossStart:
                    StartBoss();
                    break;

                case TriggerBoxKind.BossStartRoaming:
                    StartRoaming();
                    break;
                
                case TriggerBoxKind.StopMovement:
                    // StopBossMovement();
                    break;
                
            }
            
        }
    }

    private void StartRoaming()
    {
        BossEvents.StartRoaming.Invoke();
        
    }
    
    private void StopBossMovement()
    {
        BossEvents.StopBossMovement.Invoke();
        
    }

    private void StartBoss()
    {
        BossEvents.BossStart.Invoke();
    }

    private void PlayerDied()
    {
        StopBossMovement();
        _canWork = true;
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class FMODEvents : MonoBehaviour
{

    public static FMODEvents instance { get; private set; }
    
    [field: Header("Ambience")]
    [field: Header("Music")]
    
    
    [field: SerializeField] public EventReference firstLevelMusic { get; private set; }
    [field: SerializeField] public EventReference secondLevelMusic { get; private set; }
    [field: SerializeField] public EventReference thirdLevelMusic { get; private set; }
    [field: SerializeField] public EventReference BossLevelMusic { get; private set; }

    [field: SerializeField] public EventReference ThinkMusic { get; private set; }
    [field: SerializeField] public EventReference ChickenMusic { get; private set; }
    [field: SerializeField] public EventReference JonWhickStoryMusic { get; private set; }
    
    [field: SerializeField] public EventReference chicks { get; private set; }


    [field: Header("Player SFX")]
    [field: SerializeField]
    public EventReference playerFootsteps { get; private set; }
    [field: SerializeField]public EventReference playerTurnSide { get; private set; }
    [field: SerializeField]public EventReference playerJump { get; private set; }
    [field: SerializeField]public EventReference playerWallSlide { get; private set; }
    [field: SerializeField]public EventReference playerGliding { get; private set; }
    [field: SerializeField]public EventReference playerHeartbeat { get; private set; }
    [field: SerializeField]public EventReference playerGotHurt { get; private set; }

    [field: SerializeField]public EventReference playerDie { get; private set; }
    [field: SerializeField]public EventReference playerWakeUp { get; private set; }
    [field: SerializeField]public EventReference playerLanding { get; private set; }

    
    [field: Header("Feathers SFX")]
    [field: SerializeField]
    public EventReference collectFeatherSound { get; private set; }
    [field: SerializeField] public EventReference idleFeatherSound { get; private set; }
    
    
    [field: SerializeField] public EventReference windSound { get; private set; }

    [field: Header("Platforms")][field: SerializeField] public EventReference crumblingPlatform { get; private set; }
    [field: SerializeField] public EventReference fallingPlatform { get; private set; }
    [field: SerializeField] public EventReference woodPlatform { get; private set; }
    [field: SerializeField] public EventReference movingThrowPlatform { get; private set; }
    [field: SerializeField] public EventReference returnPlatform { get; private set; }
    
    [field: Header("Cave SFX")]
    [field: SerializeField] public EventReference caveAppear { get; private set; }
    [field: SerializeField] public EventReference rocksFall { get; private set; }
    
    [field: Header("Camara SFX")]
    [field: SerializeField] public EventReference camMovement { get; private set; }
    [field: SerializeField] public EventReference camZoom { get; private set; }

    [field: Header("End Level")]
    [field: SerializeField] public EventReference gateOpen { get; private set; }
    
    [field: Header("Feathers SFX")]
    [field: SerializeField] public EventReference buttonsMove { get; private set; }
    
    [field: Header("Boss")]
    [field: SerializeField] public EventReference crowYellShort { get; private set; }
    [field: SerializeField] public EventReference crowYellLong { get; private set; }
    [field: SerializeField] public EventReference crowFly { get; private set; }
    [field: SerializeField] public EventReference rockThrow { get; private set; }
    [field: SerializeField] public EventReference rockHit { get; private set; }
    [field: SerializeField] public EventReference startAttack { get; private set; }
    [field: SerializeField] public EventReference crowGetHurt { get; private set; }
    
    [field: Header("ChickPoints")]
    [field: SerializeField] public EventReference openChickPoint { get; private set; }
    
    [field: Header("Enemies")]
    [field: SerializeField] public EventReference boar { get; private set; }
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one FMOD Events instance in the scene.");
        }

        instance = this;
    }

    

    
    
}
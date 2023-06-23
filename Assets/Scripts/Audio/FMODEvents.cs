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
    [field: SerializeField]
    public EventReference Music { get; private set; }


    [field: Header("Player SFX")]
    [field: SerializeField]
    public EventReference playerFootsteps { get; private set; }

    [field: Header("Feathers SFX")]
    [field: SerializeField]
    public EventReference collectFeatherSound { get; private set; }
    [field: SerializeField] public EventReference idleFeatherSound { get; private set; }
    
    
    [field: SerializeField] public EventReference windSound { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one FMOD Events instance in the scene.");
        }

        instance = this;
    }
    
}
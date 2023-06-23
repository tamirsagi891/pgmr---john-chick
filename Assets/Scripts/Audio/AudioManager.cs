using System;
using System.Collections;
using System.Collections.Generic;
using BitStrap;
using Elad.Music;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using Mechanics.UI.Menus;
using UnityEditor;
using Logger = Nemesh.Logger;

public class AudioManager : MonoBehaviour
{
    [Header("Volume")]
    [Range(0, 1)]
    public float masterVolume = 1;
    [Range(0, 1)]
    public float musicVolume = 1;
    [Range(0, 1)]
    public float ambienceVolume = 1;
    [Range(0, 1)]
    public float SFXVolume = 1;

    private Bus masterBus;
    private Bus musicBus;
    private Bus ambienceBus;
    private Bus sfxBus;

    private List<EventInstance> eventInstances;
    private List<StudioEventEmitter> eventEmitters;

    private EventInstance ambienceEventInstance;
    private EventInstance musicEventInstance;

    public static AudioManager instance { get; private set; }

    public SoundsData Data
    {
        get => soundsData;
        set => soundsData = value;
    }

    [SerializeField] private SoundsData soundsData;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one Audio Manager in the scene.");
        }
        instance = this;

        eventInstances = new List<EventInstance>();
        eventEmitters = new List<StudioEventEmitter>();

        masterBus = RuntimeManager.GetBus("bus:/");
        musicBus = RuntimeManager.GetBus("bus:/Music");
        // ambienceBus = RuntimeManager.GetBus("bus:/Ambience");
        // sfxBus = RuntimeManager.GetBus("bus:/SFX");
    }

    private void Start()
    {
        InitializeMusic(FMODEvents.instance.Music);
    }

    private void OnEnable()
    {
        if (MenuManager.Menu == null)
        {
            return;
        }
        MenuManager.Menu.soundEvents.onMasterChange.AddListener(SetMasterVolume);
        MenuManager.Menu.soundEvents.onMasterChange.AddListener(SetMusicVolume);
    }

    private void OnDisable()
    {
        if (MenuManager.Menu == null)
        {
            return;
        }
        MenuManager.Menu.soundEvents.onMasterChange.RemoveListener(SetMasterVolume);
        MenuManager.Menu.soundEvents.onMusicChange.RemoveListener(SetMusicVolume);
    }

    private void Update()
    {
        masterBus.setVolume(masterVolume);
        musicBus.setVolume(musicVolume);
        // ambienceBus.setVolume(ambienceVolume);
        // sfxBus.setVolume(SFXVolume);
    }
    
    private void InitializeMusic(EventReference musicEventReference)
    {
        musicEventInstance = CreatEventInstance(musicEventReference);
        // musicEventInstance.start();
    }
    

    public void PlayOneShot(EventReference sound, Vector3 worldPos)
    {
        RuntimeManager.PlayOneShot(sound, worldPos);
    }

    public EventInstance CreatEventInstance(EventReference eventReference)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
        eventInstances.Add(eventInstance);
        return eventInstance;
    }

    public StudioEventEmitter InitializeEventEmitter(EventReference eventReference, GameObject emitterGameObject)
    {
        StudioEventEmitter emitter = emitterGameObject.GetComponent<StudioEventEmitter>();
        emitter.EventReference = eventReference;
        eventEmitters.Add(emitter);
        return emitter;
    }

    
    private void CleanUp()
    {
        // stop and release any created instances
        foreach (EventInstance eventInstance in eventInstances)
        {
            eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            eventInstance.release();
        }
        // stop all of the event emitters, because if we don't they may hang around in other scenes
        foreach (StudioEventEmitter emitter in eventEmitters)
        {
            emitter.Stop();
        }
    }

    private void OnDestroy()
    {
        CleanUp();
    }

    #region Event Listeners

    public void SetMasterVolume(float volume)
    {
        masterVolume = volume;
        masterBus.setVolume(masterVolume);
    }
    
    public void SetMusicVolume(float volume)
    {
        musicVolume = volume;
        musicBus.setVolume(volume);
    }

    #endregion


    [Button]
    public void MakeSound()
    {
        AudioManager.instance.PlayOneShot(FMODEvents.instance.collectFeatherSound, this.transform.position);
    }
}
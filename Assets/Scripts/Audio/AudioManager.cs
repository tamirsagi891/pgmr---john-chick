using System;
using System.Collections.Generic;
using BitStrap;
using Elad.Music;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using JetBrains.Annotations;
using Mechanics.UI.Menus;
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

    [SerializeField]
    private SoundsData soundsData;

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

    private void OnValidate()
    {
        masterBus.setVolume(masterVolume);
        musicBus.setVolume(musicVolume);
        // ambienceBus.setVolume(ambienceVolume);
        // sfxBus.setVolume(SFXVolume);
    }

    private void Start()
    {
        InitializeMusic(FMODEvents.instance.Music);
    }

    private void OnEnable()
    {
        MenuManager.OnMasterChangeEvent += SetMasterVolume;
        MenuManager.OnMusicChangeEvent += SetMusicVolume;
        MenuManager.OnSfxChangeEvent += SetSfxVolume;
        MenuManager.OnAmbientChangeEvent += SetAmbientVolume;
    }

    private void OnDisable()
    {
        MenuManager.OnMasterChangeEvent -= SetMasterVolume;
        MenuManager.OnMusicChangeEvent -= SetMusicVolume;
        MenuManager.OnSfxChangeEvent -= SetSfxVolume;
        MenuManager.OnAmbientChangeEvent -= SetAmbientVolume;
    }

    // private void Update()
    // {
    //     masterBus.setVolume(masterVolume);
    //     musicBus.setVolume(musicVolume);
    //     // ambienceBus.setVolume(ambienceVolume);
    //     sfxBus.setVolume(SFXVolume);
    // }

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

    public void SetMasterVolume([CanBeNull] object caller, float volume)
    {
        masterVolume = volume;
        masterBus.setVolume(masterVolume);
    }

    public void SetMusicVolume([CanBeNull] object caller, float volume)
    {
        musicVolume = volume;
        musicBus.setVolume(volume);
    }

    public void SetSfxVolume([CanBeNull] object sender, float volume)
    {
        SFXVolume = volume;
        sfxBus.setVolume(volume);
    }

    public void SetAmbientVolume([CanBeNull] object sender, float volume)
    {
        ambienceVolume = volume;
        ambienceBus.setVolume(volume);
    }

    #endregion


    [Button]
    public void MakeSound()
    {
        AudioManager.instance.PlayOneShot(FMODEvents.instance.collectFeatherSound, this.transform.position);
    }
}
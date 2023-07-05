using System;
using System.Collections.Generic;
using BitStrap;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using JetBrains.Annotations;
using Mechanics.UI.Menus;
using Logger = Nemesh.Logger;
using STOP_MODE = FMOD.Studio.STOP_MODE;

public class AudioManager : MonoBehaviour
{
    [Header("Volume")] [SerializeField] [Range(0, 1)]
    private float master;

    [SerializeField] [Range(0, 1)] private float music;
    [SerializeField] [Range(0, 1)] private float ambience;
    [SerializeField] [Range(0, 1)] private float sfx;

    private void Update()
    {
        MasterVolume = master;
        MusicVolume = music;
        AmbienceVolume = ambience;
        SfxVolume = sfx;
    }


    [Range(0, 1)] private float masterVolume = 1;

    [Range(0f, 1f)] private float musicVolume = 0.2f;

    [Range(0, 1)] private float ambienceVolume = 1;

    [Range(0, 1)] private float SFXVolume = 1;


    private Bus masterBus;
    private Bus musicBus;
    private Bus ambienceBus;
    private Bus sfxBus;

    private List<EventInstance> eventInstances;
    private List<StudioEventEmitter> eventEmitters;

    private EventInstance ambienceEventInstance;
    private EventInstance mainMusic;


    public static AudioManager instance { get; private set; }


    public float MasterVolume
    {
        get => masterVolume;
        set
        {
            masterVolume = value;
            masterBus.setVolume(MasterVolume);
        }
    }

    public float MusicVolume
    {
        get => musicVolume;
        set
        {
            musicVolume = value;
            musicBus.setVolume(MusicVolume);
        }
    }

    public float AmbienceVolume
    {
        get => ambienceVolume;
        set
        {
            ambienceVolume = value;
            ambienceBus.setVolume(AmbienceVolume);
        }
    }

    public float SfxVolume
    {
        get => SFXVolume;
        set
        {
            SFXVolume = value;
            sfxBus.setVolume(SfxVolume);
        }
    }


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
        ambienceBus = RuntimeManager.GetBus("bus:/Ambiance");
        sfxBus = RuntimeManager.GetBus("bus:/SFX");
    }

    private void OnValidate()
    {
        masterBus.setVolume(MasterVolume);
        musicBus.setVolume(MusicVolume);
        ambienceBus.setVolume(AmbienceVolume);
        sfxBus.setVolume(SfxVolume);
    }

    private void Start()
    {
        InitializeMusic(FMODEvents.instance.Music);
        InitializeAmbience(FMODEvents.instance.windSound);
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


    private void InitializeMusic(EventReference musicEventReference)
    {
        mainMusic = CreatEventInstance(musicEventReference);
        mainMusic.start();
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
        MasterVolume = volume;
    }

    public void SetMusicVolume([CanBeNull] object caller, float volume)
    {
        MusicVolume = volume;
    }

    public void SetSfxVolume([CanBeNull] object sender, float volume)
    {
        SfxVolume = volume;
    }

    public void SetAmbientVolume([CanBeNull] object sender, float volume)
    {
        AmbienceVolume = volume;
    }

    #endregion

    private void InitializeAmbience(EventReference ambienceEventReference)
    {
        ambienceEventInstance = CreatEventInstance(ambienceEventReference);
        ambienceEventInstance.start();
    }

    public void SetAmbienceParameter(string parameterName, float parameterValue)
    {
        ambienceEventInstance.setParameterByName(parameterName, parameterValue);
    }

    public void SetMusicParameter(string parameterName, float parameterValue)
    {
        mainMusic.setParameterByName(parameterName, parameterValue);
    }


    public void SetMusicArea(MusicStrings.AreaSound areaSound)
    {
        mainMusic.setParameterByName(MusicStrings.areaParam, (float) areaSound);
    }

    private MusicStrings.AreaSound _currAreaSound = MusicStrings.AreaSound.OpenField;

    [Button]
    public void ChangeArea()
    {
        if (_currAreaSound == MusicStrings.AreaSound.OpenField)
        {
            _currAreaSound = MusicStrings.AreaSound.Cave;
        }

        else
        {
            _currAreaSound = MusicStrings.AreaSound.OpenField;
        }

        SetMusicArea(_currAreaSound);
    }


    [Button]
    public void ChangeMusicVolume()
    {
        Logger.Log(MusicVolume);
        Logger.Log(MusicStrings.musicVol);
        SetMusicParameter(MusicStrings.musicVol, MusicVolume);
    }


    [Button]
    public void OpenMainMusic()
    {
        mainMusic.start();
    }

    [Button]
    public void CloseMainMusic()
    {
        mainMusic.stop(STOP_MODE.ALLOWFADEOUT);
    }
    
    public void ButtonSound()
    {
        AudioManager.instance.PlayOneShot(FMODEvents.instance.buttonsMove, transform.position);
    }
}
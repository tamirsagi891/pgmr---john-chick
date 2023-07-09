using System;
using System.Collections.Generic;
using BitStrap;
using Elad.Events;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using JetBrains.Annotations;
using Mechanics.UI.Menus;
using Logger = Nemesh.Logger;
using STOP_MODE = FMOD.Studio.STOP_MODE;

public class AudioManager : MonoBehaviour
{
    
    [Header("Volume")]
    [SerializeField]
    [Range(0, 1)]
    private float masterVolume = 1;

    [SerializeField]
    [Range(0f, 1f)]
    private float musicVolume = 0.2f;

    [SerializeField]
    [Range(0, 1)]
    private float ambienceVolume = 1;

    [Range(0, 1)]
    [SerializeField]
    private float SFXVolume = 1;


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

    public bool CanPlayCamSounds
    {
        get => canPlayCamSounds;
        set => canPlayCamSounds = value;
    }


    private List<EventInstance> oneShotSounds;

    [Header("Do not hear sounds")] [SerializeField]
    private bool canPlayCamSounds;

    [Header("Big Levels Music")] [SerializeField]
    private LevelSound _levelSound = LevelSound.One;  
    enum LevelSound
    {
        One,
        Two,
        Three,
        Boss
    }
    
    private void OnValidate()
    {
        MasterVolume = masterVolume;
        MusicVolume = musicVolume;
        AmbienceVolume = ambienceVolume;
        SfxVolume = SFXVolume;
    }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one Audio Manager in the scene.");
        }

        instance = this;
        oneShotSounds = new List<EventInstance>();
        eventInstances = new List<EventInstance>();
        eventEmitters = new List<StudioEventEmitter>();

        masterBus = RuntimeManager.GetBus("bus:/");
        musicBus = RuntimeManager.GetBus("bus:/Music");
        ambienceBus = RuntimeManager.GetBus("bus:/Ambiance");
        sfxBus = RuntimeManager.GetBus("bus:/SFX");

        MusicVolume = musicVolume;

#if UNITY_EDITOR && !NEMESH_EDITOR
        MasterVolume = masterVolume;
        MusicVolume = musicVolume;
        AmbienceVolume = ambienceVolume;
        SfxVolume = SFXVolume;
#endif
    }

    private void Start()
    {
        switch (_levelSound)
        {
            case LevelSound.One:
                InitializeMusic(FMODEvents.instance.firstLevelMusic);
                break;
            case LevelSound.Two:
                InitializeMusic(FMODEvents.instance.secondLevelMusic);
                break;
            case LevelSound.Three:
                InitializeMusic(FMODEvents.instance.thirdLevelMusic);
                break;
            case LevelSound.Boss:
                InitializeMusic(FMODEvents.instance.BossLevelMusic);
                break;
        }
        
        InitializeAmbience(FMODEvents.instance.windSound);
    }

    private void OnEnable()
    {
        MenuManager.OnMasterChangeEvent += SetMasterVolume;
        MenuManager.OnMusicChangeEvent += SetMusicVolume;
        MenuManager.OnSfxChangeEvent += SetSfxVolume;
        MenuManager.OnAmbientChangeEvent += SetAmbientVolume;

        // characterEvents.PauseGame.AddListener(PauseSounds);
        // characterEvents.ContinueGame.AddListener(ContinueSounds);
    }

    private void OnDisable()
    {
        MenuManager.OnMasterChangeEvent -= SetMasterVolume;
        MenuManager.OnMusicChangeEvent -= SetMusicVolume;
        MenuManager.OnSfxChangeEvent -= SetSfxVolume;
        MenuManager.OnAmbientChangeEvent -= SetAmbientVolume;

        // characterEvents.PauseGame.RemoveListener(PauseSounds);
        // characterEvents.ContinueGame.RemoveListener(ContinueSounds);
    }


    private void InitializeMusic(EventReference musicEventReference)
    {
        
        mainMusic = CreatEventInstance(musicEventReference);
        mainMusic.start();
    }

    public void AddEmitter(StudioEventEmitter newEmitter)
    {
        eventEmitters.Add(newEmitter);
    }

    public void PlayOneShot(EventReference sound, Vector3 worldPos)
    {
        var currentSoundInstance = CreatEventInstance(sound);
        oneShotSounds.Add(currentSoundInstance);
        currentSoundInstance.start();
    }

    private void ContinueSounds()
    {
        foreach (var eventEmitter in eventEmitters)
        {
            eventEmitter.EventInstance.setPaused(false);
        }

        foreach (var eventInstance in eventInstances)
        {
            eventInstance.setPaused(false);
        }

        foreach (var currentSoundInstance in oneShotSounds)
        {
            if (currentSoundInstance.isValid())
            {
                currentSoundInstance.setPaused(false);
            }
        }
    }

    private void PauseSounds()
    {
        foreach (var eventEmitter in eventEmitters)
        {
            eventEmitter.EventInstance.setPaused(true);
        }

        foreach (var eventInstance in eventInstances)
        {
            eventInstance.setPaused(true);
        }

        foreach (var currentSoundInstance in oneShotSounds)
        {
            if (currentSoundInstance.isValid())
            {
                currentSoundInstance.setPaused(true);
            }

            else
            {
                oneShotSounds.Remove(currentSoundInstance);
            }
        }
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
        mainMusic.setParameterByName(MusicStrings.areaParam, (float)areaSound);
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
        PlayOneShot(FMODEvents.instance.buttonsMove, transform.position);
    }

    public void SetBossMusic(int number)
    {
        var song = MusicStrings.firstMusic;
        switch (number)
        {
            case 2:
                song = MusicStrings.secondMusic;
                break;
            
            case 3:
                song = MusicStrings.thirdMusic;
                break;
            
            case 4:
                song = MusicStrings.fourMusic;
                break;
        }

        mainMusic.setParameterByNameWithLabel(MusicStrings.BossMusic, song);
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using BitStrap;
using Cinemachine;
using Elad.Events;
using Elad.Scripts;
using Managers;
using Mechanics.Black_Feather;
using Mechanics.UI.Menus;
using Unity.VisualScripting;
using UnityEngine;
using Logger = Nemesh.Logger;

public class EndLevelCutScene : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera cutSceneCamara;
    private CinemachineBasicMultiChannelPerlin _channelPerlin;

    private bool _startCutScene;
    private bool _openMenu;

    private BlackFeather _blackFeather;
    [SerializeField] private float openMenuTime = 1f;

    private bool _startMovement;
    [SerializeField] private float startMovementTime = 1f;
    [SerializeField] [Range(0, 1)] private float timeScale = 0.5f;

    private bool _isShaking;

    enum CutSceneOption
    {
        Regular,
        Boss
    }

    [SerializeField] private CutSceneOption _cutSceneOption = CutSceneOption.Regular;

    public bool StartCutScene
    {
        get => _startCutScene;
        set
        {
            _startCutScene = value;
            PlayerStatus.InCutScene = value;
            if (value)
            {
                _startMovement = true;
                PlayerStatus.InCutScene = true;
                PlayerStatus.Player.GetComponent<PlayerController>().CloseMovement();
                cutSceneCamara.enabled = true;
                Time.timeScale = timeScale;
            }
        }
    }

    [Header("Screen Shack")] [SerializeField]
    private float shakeIntensity = 5;

    [SerializeField] private float shakeTime = 5;

    private void Awake()
    {
        _blackFeather = GetComponentInChildren<BlackFeather>();
        SetComponents();
    }

    private void SetComponents()
    {
        _channelPerlin = cutSceneCamara.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    private void Update()
    {
        if (_cutSceneOption == CutSceneOption.Regular)
        {
            if (_openMenu)
            {
                openMenuTime -= Time.deltaTime;
                if (openMenuTime <= 0)
                {
                    _openMenu = false;
                    MenuManager.Menu.OpenEndLevelMenu();
                }
            }
        }

        if (_startMovement)
        {
            startMovementTime -= Time.deltaTime;
            if (startMovementTime <= 0)
            {
                _startMovement = false;
                StopMovement();
                PlayerStatus.Player.GetComponent<HorizontalMovement>().MoveRight();
                _openMenu = true;
            }
        }

        if (_cutSceneOption == CutSceneOption.Boss && _startCutScene)
        {
            if (_blackFeather.GotHit)
            {
                StopMovement();
                DoShake(shakeTime, shakeIntensity);
            }


            if (_isShaking && cutSceneCamara)
            {
                shakeTime -= Time.fixedDeltaTime;
                if (shakeTime <= 0)
                {
                    _isShaking = false;
                    _channelPerlin.m_AmplitudeGain = 0;
                    StartBoss();
                }
            }
        }
    }

    public void DoShake(float shakeTimer = -1, float shakeIntensity = -1)
    {
        // Time.timeScale = 1;
        _isShaking = true;
        shakeTime = shakeTimer;
        _channelPerlin.m_AmplitudeGain = shakeIntensity;
    }


    [Button]
    public void StopMovement()
    {
        PlayerStatus.Player.GetComponent<HorizontalMovement>().ResetMovement();
        PlayerStatus.Player.GetComponent<CharacterJump>().ResetMovement();
    }

    private void ZoomCamara()
    {
        PlayerStatus.ZoomCamera.StartZoom();
    }

    private void OnDestroy()
    {
        PlayerStatus.InCutScene = false;
        Time.timeScale = 1;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_cutSceneOption == CutSceneOption.Regular) return;

        if (other.CompareTag(TagStrings.playerTag))
        {
            StartCutScene = true;
        }
    }
    
    private void StartBoss()
    {
        StartCutScene = false;
        
        PlayerStatus.InCutScene = false;
        cutSceneCamara.enabled = false;
        PlayerStatus.CurrentVirtualCamara.enabled = true;
        Time.timeScale = 1;
        BossEvents.BossStart.Invoke();
        AudioManager.instance.SetBossMusic(2);
        Destroy(gameObject);
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using BitStrap;
using Cinemachine;
using Elad.Events;
using UnityEngine;
using Random = UnityEngine.Random;

public class CamaraShake : MonoBehaviour
{
    [SerializeField] private float intensityMax = 10;
    [SerializeField] private float intensityMin = 5;
    
    
    [SerializeField] private float shackTimeMin = 1;
    [SerializeField] private float shackTimeMax = 3;
    
    private float shackTimer;
    private bool _isShaking;

    private CinemachineVirtualCamera _cM;
    private CinemachineBasicMultiChannelPerlin _channelPerlin;

    private void Awake()
    {
        SetComponents();
    }

    private void SetComponents()
    {
        _cM = GetComponent<CinemachineVirtualCamera>();
        _channelPerlin = _cM.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        _channelPerlin.m_AmplitudeGain = 0;
    }

    private void OnEnable()
    {
        
        characterEvents.CharacterDamaged.AddListener(StartShake);
        BossEvents.CamaraShake.AddListener(DoShake);
    }

    private void OnDisable()
    {
        _channelPerlin.m_AmplitudeGain = 0;
        characterEvents.CharacterDamaged.RemoveListener(StartShake);
    }
    
    
    
    private void StartShake(GameObject gameObject, int value)
    {
        _isShaking = true;
        shackTimer = Random.Range(shackTimeMin, shackTimeMax);
        _channelPerlin.m_AmplitudeGain = Random.Range(intensityMin, intensityMax);
    }

    public void DoShake(float shakeTime = -1, float shakeIntensity = -1)
    {
        _isShaking = true;
        shackTimer = shakeTime < 0 ? Random.Range(shackTimeMin, shackTimeMax) : shakeTime;
        _channelPerlin.m_AmplitudeGain = shakeIntensity < 0 ? Random.Range(intensityMin, intensityMax) : shakeIntensity;
    }

    private void Update()
    {
        if (_isShaking && _cM)
        {
            shackTimer -= Time.deltaTime;
            if (shackTimer <= 0)
            {
                _isShaking = false;
                _channelPerlin.m_AmplitudeGain = 0;
            }
        }
    }

    

    
}
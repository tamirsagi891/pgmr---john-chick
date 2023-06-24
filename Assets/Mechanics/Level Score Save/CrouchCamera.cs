using System;
using System.Collections;
using System.Collections.Generic;
using Avrahamy;
using Cinemachine;
using UnityEngine;
using Logger = Nemesh.Logger;

[RequireComponent(typeof(CinemachineVirtualCamera))]
public class CrouchCamera : MonoBehaviour
{
    [SerializeField]
    private PassiveTimer timeToHold = new(0.15f);
    
    [SerializeField]
    private Vector3 additionalOffset = new(0f, -10f, 0f);
    
    private CinemachineVirtualCamera _myCamera;
    private Vector3 _initialOffset;
    private CinemachineFramingTransposer _myTransposer;

    public CinemachineVirtualCamera VirtualCamera
    {
        get => _myCamera;
        set => _myCamera = value;
    }


    private void Awake()
    {
        _myCamera = GetComponent<CinemachineVirtualCamera>();
        _myTransposer = _myCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        _initialOffset = _myTransposer.m_TrackedObjectOffset;
    }

    private void Update()
    {
        if (timeToHold.IsSet && !timeToHold.IsActive)
        {
            timeToHold.Clear();
            _myTransposer.m_TrackedObjectOffset = _initialOffset + additionalOffset;
            // Logger.Log("Camera Offset Change", this);
        }
    }

    public void SetOffset()
    {
        timeToHold.Start();
        // Logger.Log("Starting Camera Crouch", this);
    }

    public void ClearOffset()
    {
        _myTransposer.m_TrackedObjectOffset = _initialOffset;
        // Logger.Log("Ending Camera Crouch", this);
        timeToHold.Clear();
    }
    
    
}

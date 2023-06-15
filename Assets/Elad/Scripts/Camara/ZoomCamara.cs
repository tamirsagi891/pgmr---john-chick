using System;
using System.Collections;
using BitStrap;
using UnityEngine;
using Cinemachine;
using Elad.Events;
using Elad.Scripts;
using Mechanics.UI.Menus;
using UnityEngine.U2D;
using Logger = Nemesh.Logger;



public class ZoomCamera : MonoBehaviour
{
    private CinemachineVirtualCamera _cam;
    [SerializeField] private bool startZoomWithTimer = true;

    private bool _startZoom;
    [SerializeField] private float startZoomTime = 0.2f;
    private float _startZoomTimer;
    

    private void Awake()
    {
        _cam = GetComponent<CinemachineVirtualCamera>();
        PlayerStatus.ZoomCamera = this;

    }

    private void Start()
    {
        _cam.LookAt = PlayerStatus.Player.transform;
        _cam.Follow = PlayerStatus.Player.transform;
    }

    private void Update()
    {
        if (_startZoom)
        {
            _startZoomTimer -= Time.deltaTime;
            if (_startZoomTimer <= 0)
            {
                _startZoom = false;
                ZoomToDistance();
            }
        }
    }


    public void ZoomToDistance()
    {
        // var currentVirtualCam = PlayerStatus.CurrentVirtualCamara;
        _cam.enabled = true;
        characterEvents.OpenGameOverMenu.Invoke();
        // currentVirtualCam.enabled = false;

    }
    
    
    [Button]
    public void StartZoom()
    {
        if (startZoomWithTimer)
        {
            _startZoom = true;
            _startZoomTimer = startZoomTime;
        }
        else
        {
            ZoomToDistance();    
        }
    }

    [Button]
    public void ReturnToStartDistance()
    {
        // var currentVirtualCam = PlayerStatus.CurrentVirtualCamara;
        _cam.enabled = false;
        // currentVirtualCam.enabled = true;
    }
}

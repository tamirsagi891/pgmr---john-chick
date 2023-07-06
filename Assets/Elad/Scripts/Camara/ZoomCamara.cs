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
    [SerializeField] [Range(0, 1)] private float slowTimeTo = 0.5f;
    private CinemachineVirtualCamera _cam;
    [SerializeField] private bool startZoomWithTimer = true;

    private bool _startZoom;
    [SerializeField] private float startZoomTime = 0.2f;
    private float _startZoomTimer;

    private void OnEnable()
    {
        characterEvents.PlayerRevive.AddListener(ReturnToStartDistance);
        characterEvents.PlayerRevive.AddListener(OpenTime);
    }

    private void OnDisable()
    {
        characterEvents.PlayerRevive.RemoveListener(OpenTime);
        characterEvents.PlayerRevive.RemoveListener(ReturnToStartDistance);
    }
    

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

    [Button]
    public void SlowTime()
    {
        Time.timeScale = slowTimeTo;
    }
    
    [Button]
    public void OpenTime()
    {
        Time.timeScale = 1;
    }
    
    [Button]
    public void ZoomToDistance()
    {
        
        // var currentVirtualCam = PlayerStatus.CurrentVirtualCamara;
        AudioManager.instance.PlayOneShot(FMODEvents.instance.camZoom, transform.position);

        _cam.enabled = true;
        characterEvents.OpenGameOverMenu.Invoke();
        SlowTime();
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

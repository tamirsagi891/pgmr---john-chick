using System;
using System.Collections;
using BitStrap;
using UnityEngine;
using Cinemachine;
using Elad.Events;
using Elad.Scripts;
using Mechanics.UI.Menus;
using Logger = Nemesh.Logger;

public class ZoomCamera : MonoBehaviour
{
    private CinemachineVirtualCamera  _cam;
    private CinemachineTransposer _transpose;
    
    private float startDistance;
    [SerializeField] private float wantedDistance;
    [SerializeField] private float zoomSpeed = 0.2f;

    [SerializeField] private bool startZoomWithTimer = true;
    private bool _startZoom;
    [SerializeField] private float startZoomTime = 0.2f;
    [SerializeField] private float _startZoomTimer;
    
    
    private void OnEnable()
    {
        characterEvents.FunctionsLoad.AddListener(ReturnToStartDistance);
    }

    private void OnDisable()
    {
        characterEvents.FunctionsLoad.RemoveListener(ReturnToStartDistance);
    }
    
    private void Awake()
    {
        _cam = GetComponent<CinemachineVirtualCamera>();
        
        _cam.LookAt = PlayerStatus.Player.transform;
        _cam.Follow = PlayerStatus.Player.transform;
        
        startDistance = _cam.m_Lens.OrthographicSize;
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

    // Function to zoom the camera to a specified distance over time
    public void ZoomToDistance()
    {
        StopAllCoroutines();
        StartCoroutine(ZoomCoroutine());
    }

    private IEnumerator ZoomCoroutine()
    {
        float currentDistance = _cam.m_Lens.OrthographicSize;
        float t = 0f;

        while (Mathf.Abs(currentDistance - wantedDistance) > 0.01f)
        {
            t += Time.deltaTime * zoomSpeed;
            currentDistance = Mathf.Lerp(currentDistance, wantedDistance, t);
            _cam.m_Lens.OrthographicSize = currentDistance;
            yield return null;
            
        }
        // Logger.Log("Got to the final zoom distance");
        MenuManager.Menu.OpenGameOverMenu();
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

    public void ReturnToStartDistance()
    {
        _cam.m_Lens.OrthographicSize = startDistance;
    }
}
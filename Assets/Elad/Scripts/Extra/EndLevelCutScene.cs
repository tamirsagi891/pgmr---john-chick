using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Elad.Scripts;
using Managers;
using Mechanics.UI.Menus;
using UnityEngine;
using Logger = Nemesh.Logger;

public class EndLevelCutScene : MonoBehaviour
{
    
    [SerializeField] private CinemachineVirtualCamera cutSceneCamara;
    private bool _startCutScene;
    private bool _openMenu;
    
    
    [SerializeField] private float openMenuTime = 1f;

    private bool _startMovement;
    [SerializeField] private float startMovementTime = 1f;
    [SerializeField] [Range(0, 1)] private float timeScale = 0.5f;
    
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

    private void Update()
    {
        if (_openMenu)
        {
            // cutSceneCamara.Follow = null;
            // cutSceneCamara.m_Follow = null;
            // cutSceneCamara.LookAt = null;
            openMenuTime -= Time.deltaTime;
            if (openMenuTime <= 0)
            {
                _openMenu = false;
                MenuManager.Menu.OpenEndLevelMenu();
                // PlayerStatus.Player.gameObject.SetActive(false);
                // PlayerStatus.Player.GetComponent<HorizontalMovement>()
            }
        }

        if (_startMovement)
        {
            startMovementTime -= Time.deltaTime;
            if (startMovementTime <= 0)
            {
                _startMovement = false;
                PlayerStatus.Player.GetComponent<HorizontalMovement>().MoveRight();
                _openMenu = true;
            }
        }
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
}

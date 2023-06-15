using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Elad.Scripts;
using Mechanics.UI.Menus;
using UnityEngine;
using Logger = Nemesh.Logger;

public class EndLevelCutScene : MonoBehaviour
{

    [SerializeField] private CinemachineVirtualCamera cutSceneCamara;
    private bool _startCutScene;
    private bool _openMenu;
    [SerializeField] private float openMenuTime = 1f;
    

    public bool StartCutScene
    {
        get => _startCutScene;
        set
        {
            _startCutScene = value;
            PlayerStatus.InCutScene = value;
            if (value)
            {
                _openMenu = true;
                cutSceneCamara.enabled = true;
            }
        }
    }

    private void Update()
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


    private void ZoomCamara()
    {
        PlayerStatus.ZoomCamera.StartZoom();
    }
    
}

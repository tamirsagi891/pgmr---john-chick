using System;
using BitStrap;
using Elad.Events;
using Elad.Scripts;
using Elad.Scripts.Combat;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Logger = Nemesh.Logger;

namespace Mechanics.UI.Menus
{
    [AddComponentMenu("Menus/Game Over Menu")]
    public class GameOverMenu : BaseMenuController
    {
        [Header("Menu Screen")] [SerializeField] [Range(0.5f, 5f)]
        private float openScreenTime = 3f;

        private float _openScreenTimer;
        private bool _openScreen;

        [Header("Zoom Call")] [SerializeField] [Range(0.5f, 5f)]
        private float startZoomTime = 3f;

        [SerializeField] private bool openWithTimer;
        private float _startZoomTimer;
        private bool _startZoom;
        private ZoomCamera _zoomCamera;


        [SerializeField] private bool lightDeath;

        private Light2D globalLight;
        private Light2D redLight;
        private Light2D playerEnvironmentLight;

        private bool _startOpenScreen;
        
        private void OnEnable()
        {
            characterEvents.FunctionsLoad.AddListener(RestartLights);
            characterEvents.PlayerDied.AddListener(PlayerDied);
            characterEvents.OpenGameOverMenu.AddListener(StartOpenScreen);
        }

        private void OnDisable()
        {
            characterEvents.PlayerDied.RemoveListener(PlayerDied);
            characterEvents.OpenGameOverMenu.RemoveListener(StartOpenScreen);
            characterEvents.FunctionsLoad.RemoveListener(RestartLights);
        }

        private void Start()
        {
            SetLights();
        }

        private void SetLights()
        {
            globalLight = LightsStatus.GlobalLight;
            redLight = LightsStatus.RedLightPlayer;
            playerEnvironmentLight = LightsStatus.EnvironmentLightPlayer;
        }

        private void Update()
        {
            if (_startOpenScreen && lightDeath)
            {
                MakeScreenDark();
            }

            if (_openScreen)
            {
                _openScreenTimer -= Time.deltaTime;
                if (_openScreenTimer <= 0)
                {
                    _openScreen = false;
                    OpenMenu();
                    
                }
            }

            if (_startZoom)
            {
                _startZoomTimer -= Time.deltaTime;
                if (_startZoomTimer <= 0)
                {
                    _startZoom = false;
                    
                    MenuManager.Menu.OpenGameOverMenu();
                }
            }
        }

        private void PlayerDied()
        {
            if (openWithTimer)
            {
                _startZoom = true;
                _startZoomTimer = startZoomTime;
            }

            else
            {
                if (_zoomCamera)
                {
                    _zoomCamera.StartZoom();
                }
                else
                {
                    _zoomCamera = PlayerStatus.ZoomCamera;
                    _zoomCamera.StartZoom();
                }
            }
        }


        public void StartOpenScreen()
        {
            Logger.Log("in start open screen");
            _startOpenScreen = true;
            _openScreenTimer = openScreenTime;
        }

        private void MakeScreenDark()
        {
            if (globalLight && redLight)
            {
                globalLight.intensity -= (Time.fixedUnscaledDeltaTime / 2);
                redLight.intensity += (Time.fixedUnscaledDeltaTime / 2);
                playerEnvironmentLight.intensity += (Time.fixedUnscaledDeltaTime / 2);
                if (globalLight.intensity <= 0)
                {
                    _openScreen = true;
                    _startOpenScreen = false;
                }
            }

            else
            {
                _startOpenScreen = false;
                OpenMenu();
            }
        }


        public override void OpenMenu()
        {
            Logger.Log("in open menu function");
            if (PlayerStatus.PlayerDamageable.CheckPointsLives > 0)
            {
                PlayerStatus.PlayerDamageable.CheckPointsLives -= 1;
                PlayerStatus.PlayerDamageable.DeathAmounts += 1;
                MenuManager.Menu.ReturnToLastCheckPoint();
            }
            else
            {
                base.OpenMenu();
                Logger.Log("tuer menu");
            }
        }

        private void RestartLights()
        {
            
            globalLight.intensity = 1;
            redLight.intensity = 0;
            playerEnvironmentLight.intensity = 0; 
        }
    }
}
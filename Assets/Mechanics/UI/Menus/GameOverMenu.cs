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


        [SerializeField] private float timeNotWithTimeScale = 0;
        private bool _startTimerWithNeTomeScale;

        [SerializeField] private Light2D globalLight;
        [SerializeField] private Light2D redLight;
        
        private void OnEnable()
        {
            characterEvents.PlayerDied.AddListener(PlayerDied);
            characterEvents.OpenGameOverMenu.AddListener(StartOpenScreen);
        }

        private void OnDisable()
        {
            characterEvents.PlayerDied.RemoveListener(PlayerDied);
            characterEvents.OpenGameOverMenu.RemoveListener(StartOpenScreen);
        }

        private void Update()
        {
            if (_startTimerWithNeTomeScale)
            {
                MakeScreenDark();
            }
            
            if (_startTimerWithNeTomeScale)
            {
                timeNotWithTimeScale += Time.fixedUnscaledDeltaTime;
            }
            
            if (_openScreen)
            {
                _openScreenTimer -= Time.deltaTime;
                if (_openScreenTimer <= 0)
                {
                    // OpenMenu();
                    // _startTimerWithNeTomeScale = true;
                    _openScreen = false;
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

        [Button]
        public void CloseTimeAndSetTimerNotWithTime()
        {
            Time.timeScale = 0;
            _startTimerWithNeTomeScale = true;
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
            _openScreen = true;
            _openScreenTimer = openScreenTime;
            _startTimerWithNeTomeScale = true;

        }

        private void MakeScreenDark()
        {
            globalLight.intensity -= (Time.fixedUnscaledDeltaTime / 2);
            redLight.intensity += (Time.fixedUnscaledDeltaTime / 2);
            if (globalLight.intensity <= 0 )
            {
                _startTimerWithNeTomeScale = false;
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
    }

    
}
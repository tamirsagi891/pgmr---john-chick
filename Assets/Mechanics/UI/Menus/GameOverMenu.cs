using Elad.Events;
using Elad.Scripts;
using Elad.Scripts.Combat;
using UnityEngine;
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
            if (_openScreen)
            {
                _openScreenTimer -= Time.deltaTime;
                if (_openScreenTimer <= 0)
                {
                    OpenMenu();
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
            _openScreen = true;
            _openScreenTimer = openScreenTime;
        }

        public override void OpenMenu()
        {
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
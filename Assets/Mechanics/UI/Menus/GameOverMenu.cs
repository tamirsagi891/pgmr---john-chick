using System;
using Elad.Events;
using Elad.Scripts;
using Unity.VisualScripting;
using UnityEngine;
using Logger = Nemesh.Logger;

namespace Mechanics.UI.Menus
{
    [AddComponentMenu("Menus/Game Over Menu")]
    public class GameOverMenu : BaseMenuController
    {
        [Space] [Header("GameOver Menu")] [SerializeField] [Range(0.5f, 5f)]
        private float openGameOverMenuTime = 3f;

        [SerializeField] bool openWithTimer = false;
        private float _openGameOverMenuTimer;
        private bool openMenu;

        private void Awake()
        {
            PlayerStatus.GameOverMenu = this;
        }

        private void OnEnable()
        {
            characterEvents.PlayerDied.AddListener(PlayerDied);
        }

        private void OnDisable()
        {
            PlayerStatus.GameOverMenu = null;
            characterEvents.PlayerDied.RemoveListener(PlayerDied);
        }

        private void Update()
        {
            if (openMenu)
            {
                _openGameOverMenuTimer -= Time.deltaTime;
                if (_openGameOverMenuTimer <= 0)
                {
                    openMenu = false;
                    MenuManager.Menu.OpenGameOverMenu();
                }
            }
        }

        private void PlayerDied()
        {
            if (openWithTimer)
            {
                openMenu = true;
                _openGameOverMenuTimer = openGameOverMenuTime;
            }

            else
            {
                PlayerStatus.CurrentVirtualCamara.GetComponent<ZoomCamera>().StartZoom();
            }
        }

        public override void OpenMenu()
        {
            base.OpenMenu();
            // Logger.Log("in open game over menu");
        }
    }
}
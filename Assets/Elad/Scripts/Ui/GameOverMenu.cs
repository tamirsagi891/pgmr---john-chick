using System;
using Elad.Events;
using Elad.Scripts.Combat;
using Mechanics.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Logger = Nemesh.Logger;

namespace Elad.Scripts.Ui
{
    public class GameOverMenu : BaseMenuController
    {
        [Space]
        [Header("GameOver Menu")]
        [SerializeField]
        [Range(0.5f, 5f)]
        private float openGameOverMenuTime = 3f;

        private float _openGameOverMenuTimer;
        private bool openMenu;

        private void OnEnable()
        {
            characterEvents.PlayerDied.AddListener(PlayerDied);
        }

        private void OnDisable()
        {
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

        public void ReturnToLastCheckPoint()
        {
            Logger.Log("in return to last check point function");
            MenuManager.Menu.CloseAllMenus();
            PlayerStatus.SaveGameManager.LoadGameFromCheckPoint();
            PlayerStatus.player.GetComponent<Damageable>().RevivePlayer();
            
        }

        private void PlayerDied()
        {
            openMenu = true;
            _openGameOverMenuTimer = openGameOverMenuTime;
        }

        public override void OpenMenu()
        {
            base.OpenMenu();
            Logger.Log("in open game over menu");
        }
    }
}
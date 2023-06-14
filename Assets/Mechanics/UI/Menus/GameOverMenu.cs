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
        [Space]
        [Header("GameOver Menu")]
        [SerializeField]
        [Range(0.5f, 5f)]
        private float openGameOverMenuTime = 3f;

        [SerializeField]
        private bool openWithTimer;

        private float _openGameOverMenuTimer;
        private bool _openMenu;

        private void Update()
        {
            if (_openMenu)
            {
                _openGameOverMenuTimer -= Time.deltaTime;
                if (_openGameOverMenuTimer <= 0)
                {
                    _openMenu = false;


                    MenuManager.Menu.OpenGameOverMenu();
                }
            }
        }

        private void OnEnable()
        {
            characterEvents.PlayerDied.AddListener(PlayerDied);
        }

        private void OnDisable()
        {
            characterEvents.PlayerDied.RemoveListener(PlayerDied);
        }

        private void PlayerDied()
        {
            if (openWithTimer)
            {
                _openMenu = true;
                _openGameOverMenuTimer = openGameOverMenuTime;
            }

            else
            {
                PlayerStatus.CurrentVirtualCamara.GetComponent<ZoomCamera>().StartZoom();
            }
        }

        public override void OpenMenu()
        {

            if (PlayerStatus.PlayerDamageable.CheckPointsLives > 0)
            {
                PlayerStatus.PlayerDamageable.CheckPointsLives -= 1;
                MenuManager.Menu.ReturnToLastCheckPoint();
            }
            else
            {
                base.OpenMenu();
                Logger.Log("in open game over menu");
            }
        }
    }
}
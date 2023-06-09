using Elad.Events;
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
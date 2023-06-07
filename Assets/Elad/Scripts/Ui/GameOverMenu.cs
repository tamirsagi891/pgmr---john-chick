using System;
using Elad.Events;
using UnityEngine;
using UnityEngine.SceneManagement;
using Logger = Nemesh.Logger;

namespace Elad.Scripts.Ui
{
    public class GameOverMenu : MonoBehaviour
    {
        [SerializeField][Range(0.5f,5f)] private float openGameOverMenuTime = 3f;
        private float _openGameOverMenuTimer;
        private bool openMenu; 
        public GameObject gameOverMenuUI;

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
                    OpenGameOverMenu();
                }
            }
        }

        public void ReturnToLastCheckPoint()
        {
            Logger.Log("in return to last check point function");
            gameOverMenuUI.SetActive(false);
            PlayerStatus.IsGamePause = false;
            PlayerStatus.SaveGameManager.LoadGameFromCheckPoint();
        }

        private void PlayerDied()
        {
            openMenu = true;
            _openGameOverMenuTimer = openGameOverMenuTime;
        }

        private void OpenGameOverMenu()
        {
            Logger.Log("in open game over menu");
            gameOverMenuUI.SetActive(true);
            PlayerStatus.IsGamePause = true;
        }
    }
}

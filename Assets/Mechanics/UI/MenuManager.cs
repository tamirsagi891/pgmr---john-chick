using System;
using BitStrap;
using Elad.Scripts;
using Elad.Scripts.Ui;
using UnityEngine;
using UnityEngine.Events;

namespace Mechanics.UI
{
    public class MenuManager : MonoBehaviour
    {
        public static MenuManager Menu { get; private set; } = null;

        #region Inspector

        [SerializeField]
        public SoundEvents soundEvents;

        [SerializeField]
        [RequiredReference]
        public SettingsMenu settingsMenu;  // TODO: Make this a class

        [SerializeField]
        [RequiredReference]
        public GameOverMenu gameOverMenu;

        [SerializeField]
        [RequiredReference]
        public PauseMenu pauseMenu;

        #endregion

        #region Public Methods

        public void OnMasterChange(float value)
        {
            soundEvents.onMasterChange.Invoke(value);
        }

        public void OnMusicChange(float value)
        {
            soundEvents.onMusicChange.Invoke(value);
        }

        public void OpenSettingsMenu()
        {
            if (_currentOpen != null)
            {
                _lastOpen = _currentOpen;
            }
            _currentOpen = settingsMenu;
            GeneralGameManager.IsGamePause = true;
            settingsMenu.OpenMenu();
            pauseMenu.CloseMenu();
            gameOverMenu.CloseMenu();
        }
        
        public void OpenPauseMenu()
        {
            if (_currentOpen != null)
            {
                _lastOpen = _currentOpen;
            }
            _currentOpen = pauseMenu;
            GeneralGameManager.IsGamePause = true;
            settingsMenu.CloseMenu();
            pauseMenu.OpenMenu();
            gameOverMenu.CloseMenu();
        }
        
        public void OpenGameOverMenu()
        {
            if (_currentOpen != null)
            {
                _lastOpen = _currentOpen;
            }
            _currentOpen = gameOverMenu;
            GeneralGameManager.IsGamePause = true;
            settingsMenu.CloseMenu();
            pauseMenu.CloseMenu();
            gameOverMenu.OpenMenu();
        }

        public void CloseAllMenus()
        {
            if (_currentOpen != null)
            {
                _lastOpen = _currentOpen;
            }
            GeneralGameManager.IsGamePause = false;
            settingsMenu.CloseMenu();
            pauseMenu.CloseMenu();
            gameOverMenu.CloseMenu();
        }

        public void BackToPreviousMenu()
        {
            if (_lastOpen != null)
            {
                if (_currentOpen != null)
                {
                    _currentOpen.CloseMenu();
                }

                _currentOpen = _lastOpen;
                GeneralGameManager.IsGamePause = true;
                _lastOpen.OpenMenu();
            }
        }

        #endregion

        #region Private Fields

        private BaseMenuController _lastOpen;
        private BaseMenuController _currentOpen;

        #endregion

        #region MonoBehaviour

        private void Awake()
        {
            if (Menu != null)
            {
                Destroy(this);
                return;
            }

            Menu = this;
            _lastOpen = pauseMenu;
            _currentOpen = null;
        }

        #endregion

    }

    [Serializable]
    public class SoundEvents
    {
        public UnityEvent<float> onMasterChange = new();
        public UnityEvent<float> onMusicChange = new();
    }
}

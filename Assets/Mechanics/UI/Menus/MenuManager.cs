using System;
using BitStrap;
using Elad.Scripts;
using Elad.Scripts.Combat;
using Managers;
using Nemesh.ScriptableObjects;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using Logger = Nemesh.Logger;


namespace Mechanics.UI.Menus
{
    [AddComponentMenu("Menus/Menus Manager", 0)]
    [RequireComponent(typeof(LoadSceneManager))]
    [SelectionBase]
    public class MenuManager : MonoBehaviour
    {
        public static MenuManager Menu { get; private set; } = null;

        #region Inspector

        [Header("Events")]
        [SerializeField]
        public SoundEvents soundEvents;

        [Header("Menus")]
        [SerializeField]
        [RequiredReference]
        public SettingsMenu settingsMenu;

        [SerializeField]
        [RequiredReference]
        public GameOverMenu gameOverMenu;

        [SerializeField]
        [RequiredReference]
        public PauseMenu pauseMenu;

        [SerializeField]
        [RequiredReference]
        public EndLevelMenu endLevelMenu;

        [Space]
        [Header("Input Management")]
        [SerializeField]
        private InputActionAsset uiInputs;

        public LoadSceneManager LoadSceneManager { get; private set; }

        #endregion

        #region Public Methods

        #region Events

        public void OnMasterChange(float value)
        {
            Logger.LogWarning("OnMasterChange: Missing save to static file.", this);
            soundEvents.onMasterChange.Invoke(value);
        }

        public void OnMusicChange(float value)
        {
            Logger.LogWarning("OnMasterChange: Missing save to static file.", this);
            soundEvents.onMusicChange.Invoke(value);
        }

        #endregion

        #region Menus

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
            endLevelMenu.CloseMenu();
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
            endLevelMenu.CloseMenu();
        }

        public void OpenGameOverMenu()
        {
            Logger.Log("in open game over menu");
            if (_currentOpen != null)
            {
                _lastOpen = _currentOpen;
            }

            _currentOpen = gameOverMenu;
            GeneralGameManager.IsGamePause = true;
            settingsMenu.CloseMenu();
            pauseMenu.CloseMenu();
            gameOverMenu.OpenMenu();
            endLevelMenu.CloseMenu();
        }

        [Button]
        public void OpenEndLevelMenu()
        {
            if (_currentOpen != null)
            {
                _lastOpen = _currentOpen;
            }

            _currentOpen = endLevelMenu;
            GeneralGameManager.IsGamePause = true;
            settingsMenu.CloseMenu();
            pauseMenu.CloseMenu();
            gameOverMenu.CloseMenu();
            endLevelMenu.OpenMenu();
        }

        [Button]
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
            endLevelMenu.CloseMenu();
            _currentOpen = null;
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

        #region Buttons

        public void Resume()
        {
            MenuManager.Menu.CloseAllMenus();
        }

        private void Pause()
        {
            MenuManager.Menu.OpenPauseMenu();
        }


        // TODO: move all of this to the MenuManager
        public void ReloadLevel()
        {
            GeneralGameManager.IsGamePause = false;
            Logger.Log("Reload Level", this);
            LoadSceneManager.ReloadScene();
        }

        public void ReturnToLastCheckPoint()
        {
            // Logger.Log("in return to last check point function", this);
            MenuManager.Menu.CloseAllMenus();
            PlayerStatus.SaveGameManager.LoadGameFromCheckPoint();
            PlayerStatus.PlayerDamageable.RevivePlayer();
        }

        public void LoadMainMenu()
        {
            GeneralGameManager.IsGamePause = false;
            Logger.Log("LoadMainMenu", this);
            LoadSceneManager.GoToScene(ScenesHolder.MainMenu);
        }

        public void LoadNextLevel()
        {
            GeneralGameManager.IsGamePause = false;
            Logger.Log("Load Next Level", this);
            LoadSceneManager.StartLoadScene();
            LoadSceneManager.GoToNext();
        }

        public void QuitGame()
        {
            GeneralGameManager.IsGamePause = false;
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }

        #endregion

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
            LoadSceneManager = GetComponent<LoadSceneManager>();
        }

        private void OnEnable()
        {
            var map = uiInputs.FindActionMap("UI");
            var pauseAction = map.FindAction("Pause");
            pauseAction.Enable();
            pauseAction.started += OnPause;
        }

        private void OnDisable()
        {
            var map = uiInputs.FindActionMap("UI");
            var pauseAction = map.FindAction("Pause");
            pauseAction.started -= OnPause;
            pauseAction.Disable();
        }

        #endregion

        #region Inputs

        public void OnPause(InputAction.CallbackContext context)
        {
            if (context.phase != InputActionPhase.Started)
            {
                return;
            }

            if (GeneralGameManager.IsGamePause && _currentOpen == pauseMenu)
            {
                Resume();
                return;
            }

            if (_currentOpen == null)
            {
                Pause();
            }
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
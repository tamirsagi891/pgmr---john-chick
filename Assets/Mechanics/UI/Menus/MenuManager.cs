using System;
using BitStrap;
using Elad.Events;
using Elad.Scripts;
using Managers;
using Mechanics.UI.Menus.Menu_Utils;
using Nemesh.ScriptableObjects;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Logger = Nemesh.Logger;


namespace Mechanics.UI.Menus
{
    [AddComponentMenu("Menus/Menus Manager", 0)]
    [RequireComponent(typeof(LoadSceneManager))]
    [SelectionBase]
    public class MenuManager : MonoBehaviour
    {
        public static MenuManager Menu { get; private set; } = null;
        
        public static event EventHandler<float> OnMasterChangeEvent;
        public static event EventHandler<float> OnMusicChangeEvent;
        public static event EventHandler<float> OnSfxChangeEvent;
        public static event EventHandler<float> OnAmbientChangeEvent;
        
        #region Inspector

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

        [SerializeField]
        [RequiredReference]
        public LevelChooser levelChooserMenu;

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
            OnMasterChangeEvent?.Invoke(this, value);
        }

        public void OnMusicChange(float value)
        {
            OnMusicChangeEvent?.Invoke(this, value);
        }
        
        public void OnSfxChange(float value)
        {
            OnSfxChangeEvent?.Invoke(this, value);
        }
        
        public void OnAmbientChange(float value)
        {
            OnAmbientChangeEvent?.Invoke(this, value);
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
            levelChooserMenu.CloseMenu();
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
            levelChooserMenu.CloseMenu();
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
            levelChooserMenu.CloseMenu();
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
            levelChooserMenu.CloseMenu();
        }
        
        [Button]
        public void OpenLevelChooser()
        {
            if (_currentOpen != null)
            {
                _lastOpen = _currentOpen;
            }

            _currentOpen = levelChooserMenu;
            GeneralGameManager.IsGamePause = true;
            settingsMenu.CloseMenu();
            pauseMenu.CloseMenu();
            gameOverMenu.CloseMenu();
            endLevelMenu.CloseMenu();
            levelChooserMenu.OpenMenu();
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
            levelChooserMenu.CloseMenu();
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
            characterEvents.ContinueGame.Invoke();
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
        
        public void ReloadLevelDark()
        {
            GeneralGameManager.LoadAsDark = true;
            GeneralGameManager.IsGamePause = false;
            Logger.Log("Reload Level Dark", Color.black, this);
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

        private void OnDestroy()
        {
            Menu = null;
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
                characterEvents.PauseGame.Invoke();
            }
            else if (_currentOpen != endLevelMenu && _currentOpen != gameOverMenu)
            {
                BackToPreviousMenu();
            }
        }

        #endregion
    }
}
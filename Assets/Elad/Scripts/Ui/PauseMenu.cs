using Managers;
using Mechanics.UI;
using Nemesh.ScriptableObjects;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Logger = Nemesh.Logger;

namespace Elad.Scripts.Ui
{
    public class PauseMenu : BaseMenuController
    {
        [Space]
        [Header("Pause Menu")]
        [SerializeField]
        public LoadSceneManager loadSceneManager;


        private void Awake()
        {
            EventSystem.current.SetSelectedGameObject(firstSelected);
        }


        public void OnPause(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                if (GeneralGameManager.IsGamePause)
                {
                    Resume();
                }
                else
                {
                    Pause();
                }
            }
        }

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
            Logger.Log("Reload Level");
            loadSceneManager.ReloadScene();
        }

        public void LoadMenu()
        {
            GeneralGameManager.IsGamePause = false;
            Logger.Log("load menu");
            loadSceneManager.GoToScene(ScenesHolder.MainMenu);
        }

        public void QuitGame()
        {
            GeneralGameManager.IsGamePause = false;
            Logger.Log("quit game");
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }
}

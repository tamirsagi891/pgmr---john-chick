using Eflatun.SceneReference;
using Elad.Events;
using Nemesh.ScriptableObjects;
using UnityEngine;
using UnityEngine.SceneManagement;
using Logger = Nemesh.Logger;

namespace Managers
{
    public static class GeneralGameManager
    {
        private static bool _isGamePause;

        public static bool IsGamePause
        {
            get => _isGamePause;
            set
            {
                _isGamePause = value;
                Time.timeScale = _isGamePause ? 0f : 1f;
                // TODO: Save the last and resume it, if we want slowmotion effect
                if (_isGamePause)
                {
                    characterEvents.PauseGame.Invoke();
                }
                else
                {
                    characterEvents.ContinueGame.Invoke();
                }

            }
        }

        public static SceneReference CurrentScene
        {
            get
            {
                var current = SceneManager.GetActiveScene();
                SceneReference currentScene = SceneReference.FromScenePath(current.path);
                return currentScene;
            }
        }

        public static int CurrentSceneIndex => ScenesHolder.Levels.FindIndex(reference => CurrentScene.Guid == reference.Guid);

        public static bool LoadAsDark { get; set; }
        public static string PlayerName { get; set; } = "Elad";

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void OnBeforeSceneLoadRuntimeMethod()
        {
            Logger.Log("Initializing Game State", Color.black);
            IsGamePause = false;
#if !UNITY_EDITOR //|| NEMESH_EDITOR
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
#endif
        }
    }
}

using Eflatun.SceneReference;
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

        public static string PlayerName { get; set; } = "Elad";

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void OnBeforeSceneLoadRuntimeMethod()
        {
            Logger.Log("Initializing Game State", Color.black);
            IsGamePause = false;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}

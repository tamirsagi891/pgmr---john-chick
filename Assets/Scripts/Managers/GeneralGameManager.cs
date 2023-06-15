using UnityEngine;
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
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void OnBeforeSceneLoadRuntimeMethod()
        {
            Logger.Log("Initializing Game State", Color.black);
            IsGamePause = false;
        }
    }
}

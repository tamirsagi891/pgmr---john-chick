using Elad.Scripts;
using UnityEngine;
using Logger = Nemesh.Logger;

namespace Mechanics.UI.Menus
{
    [AddComponentMenu("Menus/Main Menu Manager", 20)]
    public class MainMenuManager : MonoBehaviour
    {
        private void Awake()
        {
            Logger.LogWarning("We should really replace the usage of this class with MenuManager.", this);
        }

        public void QuitGame()
        {
            GeneralGameManager.IsGamePause = false;
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }

    }
}

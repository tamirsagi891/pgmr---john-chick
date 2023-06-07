using System.Collections.Generic;
using Eflatun.SceneReference;
using UnityEngine;

namespace Nemesh.ScriptableObjects
{
    [CreateAssetMenu(fileName = "SceneManager", menuName = "Scenes/Manager", order = 0)]
    public class ScenesHolder : ScriptableObject
    {
        public static ScenesHolder Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = Resources.Load<ScenesHolder>("Settings/SceneManager");
                }
                return instance;
            }
            private set => instance = value;
        }

        public static SceneReference MainMenu => Instance.mainMenu;
        public static List<SceneReference> Levels => Instance.levels;

        [SerializeField]
        public SceneReference mainMenu;

        [SerializeField]
        public List<SceneReference> levels = new();

        private static ScenesHolder instance = null;

        private void Awake()
        {
            Logger.Log("Here");
            if (Instance != null)
            {
                Logger.LogException("Error! Only a single ScenesHolder can exist at runtime.");
                Destroy(this);
                return;
            }

            Instance = this;
        }
    }
}
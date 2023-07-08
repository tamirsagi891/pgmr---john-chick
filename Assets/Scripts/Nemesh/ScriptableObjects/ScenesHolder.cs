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
        public static List<SceneReference> ExtraLevels => Instance.extraLevels;

        [SerializeField]
        public SceneReference mainMenu;


        [SerializeField]
        public List<SceneReference> levels = new();

        public List<SceneReference> extraLevels = new();

        private static ScenesHolder instance = null;

//         private void Awake()
//         {
//             if (Instance != null)
//             {
// #if UNITY_EDITOR
//                 DestroyImmediate(this);
//                 return;
// #endif
//                 Destroy(this);
//                 return;
//             }
//
//             Instance = this;
//         }
    }
}
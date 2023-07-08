using System.Collections.Generic;
using System.Text.RegularExpressions;
using Eflatun.SceneReference;
using Managers;
using Nemesh.ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace Mechanics.UI.Menus.Menu_Utils
{
    public class LevelChooser : BaseMenuController
    {
        [SerializeField]
        private LoadSceneManager loadSceneManager;

        [SerializeField]
        private GameObject container;

        [Space]
        [SerializeField]
        private Button buttonPrefab;

        public LinkedPool<Button> Pool { get; set; }

        private List<Button> _buttons = new();

        protected void Awake()
        {
            Pool = new LinkedPool<Button>(
                InitButton,
                GetButton,
                ReleaseButton,
                DestroyButton);

            // AddSceneButton(ScenesHolder.MainMenu);
            // AddSceneButton(ScenesHolder.Instance.intro);
            foreach (var level in ScenesHolder.Levels)
            {
                AddSceneButton(level);
            }
            foreach (var level in ScenesHolder.ExtraLevels)
            {
                AddSceneButton(level, extra: true);
            }
        }

        private void AddSceneButton(SceneReference sceneToAdd, bool extra = false)
        {
            var button = Pool.Get();
            button.onClick.AddListener(delegate
            {
                loadSceneManager.GoToScene(sceneToAdd);
                GeneralGameManager.IsGamePause = false;
            });
            
            var sceneName = Regex.Split(sceneToAdd.Name, @" [-] ")[0];
            if (extra)
            {
                sceneName = $"Extra: {sceneName}";
            }
            button.GetComponentInChildren<TMP_Text>().text = sceneName;
            _buttons.Add(button);
        }

        protected void DestroyButton(Button button)
        {
            Destroy(button.gameObject);
        }

        protected void ReleaseButton(Button button)
        {
            button.gameObject.SetActive(false);
        }

        protected Button InitButton()
        {
            return Instantiate(buttonPrefab, container.transform);
        }

        protected void GetButton(Button button)
        {
            button.gameObject.SetActive(true);
        }
    }
}

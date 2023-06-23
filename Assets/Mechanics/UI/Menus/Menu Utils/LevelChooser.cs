using System.Collections.Generic;
using System.Text.RegularExpressions;
using Eflatun.SceneReference;
using Managers;
using Nemesh.ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;
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
            
            AddSceneButton(ScenesHolder.MainMenu);
            AddSceneButton(ScenesHolder.Instance.intro);
            foreach (var level in ScenesHolder.Levels)
            {
                AddSceneButton(level);
            }
        }

        private void AddSceneButton(SceneReference sceneToAdd)
        {
            var button = Pool.Get();
            button.onClick.AddListener(() => loadSceneManager.GoToScene(sceneToAdd));
            var sceneName = Regex.Split(sceneToAdd.Name, @" [-] ")[0];
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
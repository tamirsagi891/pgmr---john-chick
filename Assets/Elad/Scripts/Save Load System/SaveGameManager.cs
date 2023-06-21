using System;
using Elad.Events;
using Elad.Save_Load_System;
using Managers;
using UnityEngine;
using UnityEngine.InputSystem;
using Logger = Nemesh.Logger;


namespace Elad.Scripts.Save_Load_System
{
    [DefaultExecutionOrder(100)]
    public class SaveGameManager : MonoBehaviour
    {
        private bool _firstTime = true;
        private CheckPoints _lastCheckPoint;
        [SerializeField]
        private InputActionAsset uiInputs;

        public bool FirstTime
        {
            get => _firstTime;
            set => _firstTime = value;
        }

        private void OnEnable()
        {
            characterEvents.OnJsonLoadFinish.AddListener(OnLoadFinish);
            
            var map = uiInputs.FindActionMap("Player");
            var moveAction = map.FindAction("Save"); 
            moveAction.started += SaveGameFromCheckPoint;
            moveAction.canceled += SaveGameFromCheckPoint;
        }
    
        private void OnDisable()
        {
            characterEvents.OnJsonLoadFinish.RemoveListener(OnLoadFinish);
            var map = uiInputs.FindActionMap("Player");
            var moveAction = map.FindAction("Save");
            moveAction.started -= SaveGameFromCheckPoint;
            moveAction.canceled -= SaveGameFromCheckPoint;
        }
    
        private void Awake()
        {
            PlayerStatus.SaveGameManager = this;
        }

        private void Start()
        {
            characterEvents.FunctionsSave.Invoke();
            SaveGameOnJson.SaveGame();
        }

        public void SaveGameFromCheckPoint(InputAction.CallbackContext context)
        {
            if (GeneralGameManager.IsGamePause)
            {
                return;
            }
            if (context.started && PlayerStatus.PlayerInsideCheckPoint)
            {
                characterEvents.FunctionsSave.Invoke();
                SaveGameOnJson.SaveGame();
            }
        }

        public void LoadGameFromCheckPoint(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                SaveGameOnJson.LoadGame();
            }
        }

        public void LoadGameFromCheckPoint()
        {
            Logger.Log("load game after player die");
            SaveGameOnJson.LoadGame();
        }
    

        private void OnLoadFinish()
        {
            characterEvents.FunctionsLoad.Invoke();
            characterEvents.PlayerRevive.Invoke();
            PlayerStatus.Player.transform.position = PlayerStatus.LastCheckPoint.Position;
        }
    }
}
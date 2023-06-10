using System;
using Elad.Events;
using Elad.Save_Load_System;
using UnityEngine;
using UnityEngine.InputSystem;
using Logger = Nemesh.Logger;


namespace Elad.Scripts.Save_Load_System
{
    [DefaultExecutionOrder(100)]
    public class SaveGameManager : MonoBehaviour
    {
        private CheckPoints _lastCheckPoint;
        [SerializeField] private bool canSave = true;

        private void OnEnable()
        {
            characterEvents.OnJsonLoadFinish.AddListener(OnLoadFinish);
    
        }
    
        private void OnDisable()
        {
            characterEvents.OnJsonLoadFinish.RemoveListener(OnLoadFinish);
    
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
                // Logger.Log("load game from button");
                SaveGameOnJson.LoadGame();
            }
        }

        public void LoadGameFromCheckPoint()
        {
            // Logger.Log("load game after player die");
            SaveGameOnJson.LoadGame();
        }
    

        private void OnLoadFinish()
        {
            characterEvents.FunctionsLoad.Invoke();
            PlayerStatus.player.transform.position = PlayerStatus.LastCheckPoint.Position;
        }
    }
}
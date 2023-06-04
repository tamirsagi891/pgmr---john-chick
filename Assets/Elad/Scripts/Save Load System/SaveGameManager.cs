using System;
using System.Collections;
using System.Collections.Generic;
using Elad.Events;
using Elad.Save_Load_System;
using Elad.Scripts;
using Elad.Scripts.Save_Load_System;
using UnityEngine;
using UnityEngine.InputSystem;
using Logger = Nemesh.Logger;

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
            SaveGameOnJson.LoadGame();
        }
    }

    public void LoadGameFromCheckPoint()
    {
        SaveGameOnJson.LoadGame();
    }
    

    private void OnLoadFinish()
    {
        characterEvents.FunctionsLoad.Invoke();
        PlayerStatus.player.transform.position = PlayerStatus.LastCheckPoint.Position;
    }
}
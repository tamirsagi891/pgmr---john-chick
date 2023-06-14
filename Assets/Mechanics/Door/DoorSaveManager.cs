using System;
using System.Collections.Generic;
using Elad.Events;
using Mechanics.UI.Menus.Menu_Utils;
using UnityEngine;


public class DoorSaveManager : MonoBehaviour
{
    public static CheckpointDoorsState doorsSaveState = new();

    public static List<DoorKey> DoorsToSave = new();
    
    private void OnEnable()
    {
        DoorsToSave.Clear();
        var doors = FindObjectsOfType<DoorKey>();
        DoorsToSave = new List<DoorKey>(doors);
        characterEvents.FunctionsSave.AddListener(SaveDoors);
        characterEvents.FunctionsLoad.AddListener(LoadDoors);
    }

    private void OnDisable()
    {
        characterEvents.FunctionsSave.RemoveListener(SaveDoors);
        characterEvents.FunctionsLoad.RemoveListener(LoadDoors);
        DoorsToSave.Clear();
    }
    
    private void LoadDoors()
    {
        foreach (var doorKey in DoorsToSave)
        {
            doorKey.CloseDoorImmediate();
        }
    }

    private void SaveDoors()
    {
        
    }
}

[Serializable]
public class CheckpointDoorsState
{
    
}
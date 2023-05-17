


using System;
using Elad.Scripts;
using UnityEngine;
using UnityEngine.Events;

public static class characterEvents
{
    public static UnityEvent<GameObject, int> CharacterDamaged = new();
    public static UnityEvent<GameObject, int> CharacterHealed = new();
    
    public static UnityEvent< FeathersManager.FeatherKind> AddFeather = new();
    public static UnityEvent< FeathersManager.FeatherKind> RemoveFeather = new();
}
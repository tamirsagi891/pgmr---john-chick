using System;
using Elad.Scripts;
using UnityEngine;
using UnityEngine.Events;

namespace Elad.Events
{
    public static class characterEvents
    {
        
        public static UnityEvent<GameObject, int> CharacterDamaged = new();
        public static UnityEvent<GameObject, int> CharacterHealed = new();
    
        public static UnityEvent< FeathersManager.FeatherKind> AddFeather = new();
        public static UnityEvent< FeathersManager.FeatherKind> RemoveFeather = new();
        
        public static UnityEvent< EggsManager.EggKind> AddEgg = new();
        public static UnityEvent< EggsManager.EggKind> RemoveEgg = new();
        
        public static UnityEvent<bool> playerCrouchAndJumpOnPlatform  = new();

    }
}
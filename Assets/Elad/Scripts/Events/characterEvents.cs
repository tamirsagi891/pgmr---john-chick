using System;
using Elad.Scripts;
using UnityEngine;
using UnityEngine.Events;

namespace Elad.Events
{
    public static class characterEvents
    {
        public static UnityEvent OpenGameOverMenu = new();
        
        public static UnityEvent PlayerDied = new();
        public static UnityEvent PlayerRevive = new();
        
        public static UnityEvent<GameObject, int> CharacterDamaged = new();
        public static UnityEvent<GameObject, int> CharacterHealed = new();
    
        public static UnityEvent< FeathersManager.FeatherKind> AddFeatherToPlayer = new();
        public static UnityEvent< FeathersManager.FeatherKind> RemoveFeather = new();
        
        public static UnityEvent< EggsManager.EggKind> AddEgg = new();
        public static UnityEvent< EggsManager.EggKind> RemoveEgg = new();
        
        public static UnityEvent<bool> playerCrouchAndJumpOnPlatform  = new();

        public static UnityEvent OnJsonLoadStart = new();
        public static UnityEvent OnJsonLoadFinish = new();
        
        public static UnityEvent FunctionsSave  = new();
        public static UnityEvent FunctionsLoad  = new();
        
        

    }
}
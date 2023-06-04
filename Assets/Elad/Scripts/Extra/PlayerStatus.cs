using System;
using Elad.Scripts;
using Elad.Scripts.Arrows;
using Elad.Scripts.Save_Load_System;
using UnityEngine;
using UnityEngine.Events;

namespace Elad.Scripts
{
    public static class PlayerStatus
    {
        public static bool isGamePause;
        
        public static GameObject player;
        public static int maxHealth;
        public static int curHealth;
        public static bool isFacingRight;

        public static ArrowData CurrentArrowDataData;
        public static EggData CurrentEggData;

        public static Vector2 playerVelocity;

        public static bool PlayerIsInvincible;
        public static CharacterJump JumpController { get; set; }
        public static bool IsGliding => JumpController.IsGliding;

        public static Platform PlatformController { get; set; }
        public static bool IsMovingThrowPlatform => PlatformController.IsMovingThrowPlatform;

        public static FeathersToCollectManager FeathersToCollectManager;
        public static bool InitializeFromJason;
        
        public static bool PlayerInsideCheckPoint;
        public static SaveGameManager SaveGameManager;
        public static CheckPoints LastCheckPoint;
        public static bool canSave = true;
    }
}

using System;
using Elad.Scripts;
using Elad.Scripts.Arrows;
using UnityEngine;
using UnityEngine.Events;

namespace Elad.Scripts
{
    public static class PlayerStatus
    {
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
    }
}

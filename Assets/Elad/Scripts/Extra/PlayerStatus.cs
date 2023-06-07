using System;
using Cinemachine;
using Elad.Scripts;
using Elad.Scripts.Arrows;
using Elad.Scripts.Save_Load_System;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Events;

namespace Elad.Scripts
{
    public static class PlayerStatus
    {
        public static bool IsGamePause  // TODO: Move this to another script- this isnt a PlayerStatus, its a GameState
        {
            get => isGamePause;
            set
            {
                isGamePause = value;
                Time.timeScale = isGamePause ? 0f : 1f;
                // TODO: Save the last and resume it, if we want slowmotion effect
            }
        }

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
        public static PlayerSaveData _playerSaveData = new PlayerSaveData();

        public static CinemachineVirtualCamera CurrentVirtualCamara;
        public static bool isGamePause;

        public static int CollectedFeatherAmount;
    }
    
    [System.Serializable]
    public class PlayerSaveData
    {
        public int health;
    }
}

using Cinemachine;
using Elad.Scripts.Arrows;
using Elad.Scripts.Combat;
using Elad.Scripts.Save_Load_System;
using UnityEngine;

namespace Elad.Scripts
{
    public static class PlayerStatus
    {
        public static GameObject Player
        {
            get => player;
            set
            {
                player = value;
                if (player != null)
                {
                    PlayerDamageable = player.GetComponent<Damageable>();
                }
            }
        }

        public static Damageable PlayerDamageable { get; set; }
        public static CharacterJump JumpController { get; set; }
        public static Platform PlatformController { get; set; }
        public static bool IsMovingThrowPlatform => PlatformController.IsMovingThrowPlatform;
        public static bool IsGliding => JumpController.IsGliding;
        public static bool IsGrounded;

        public static int maxHealth;
        public static int curHealth;
        public static bool IsAlive => PlayerDamageable.IsAlive;
        public static bool PlayerIsInvincible;

        public static bool isFacingRight;

        public static ArrowData CurrentArrowDataData;
        public static EggData CurrentEggData;

        public static Vector2 playerVelocity;

        public static int CollectedFeatherAmount;
        public static FeathersToCollectManager FeathersToCollectManager;

        public static bool PlayerInsideCheckPoint;

        public static bool InitializeFromJason;
        public static SaveGameManager SaveGameManager;
        public static CheckPoints LastCheckPoint;
        public static bool canSave = true;
        public static PlayerSaveData PlayerSaveData = new PlayerSaveData();

        public static CinemachineVirtualCamera CurrentVirtualCamara;
        public static ZoomCamera ZoomCamera;

        public static bool isGamePause;

        private static GameObject player;

        public static PlayerController PlayerController;
        public static bool InCutScene;
    }

    [System.Serializable]
    public class PlayerSaveData
    {
        public int health;
    }
}
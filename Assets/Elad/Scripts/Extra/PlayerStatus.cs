using System;
using Elad.Scripts;
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
    }
}

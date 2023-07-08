using System;
using Elad.Scripts;
using UnityEngine;
using UnityEngine.Events;

namespace Elad.Events
{
    public static class BossEvents
    {
        public static UnityEvent StopBossMovement = new();
        public static UnityEvent StartRoaming = new();
        public static UnityEvent BossDead = new();
        public static UnityEvent BossStart = new();
        
        public static UnityEvent< float, float> CamaraShake = new();

    }
}
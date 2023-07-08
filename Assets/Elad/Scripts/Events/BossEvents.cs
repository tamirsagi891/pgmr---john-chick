using System;
using Elad.Scripts;
using UnityEngine;
using UnityEngine.Events;

namespace Elad.Events
{
    public static class BossEvents
    {
        public static UnityEvent StartRoamingFromRunning = new();
        public static UnityEvent BossDead = new();
        public static UnityEvent BossStart = new();
        
    }
}
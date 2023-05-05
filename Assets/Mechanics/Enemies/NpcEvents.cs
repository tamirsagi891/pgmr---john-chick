using System;
using UnityEngine;
using UnityEngine.Events;

namespace Mechanics.Enemies
{
    [Serializable]
    public class NpcEvents
    {
        [SerializeField]
        public UnityEvent onJump = new();
        
        [SerializeField]
        public UnityEvent onDash = new();
        
        [SerializeField]
        public UnityEvent onPlayerDetected = new();
        
        [SerializeField]
        public UnityEvent onAttack = new();
        
        [SerializeField]
        public UnityEvent onHurt = new();

    }
}

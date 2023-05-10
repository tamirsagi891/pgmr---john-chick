using Avrahamy;
using UnityEngine;

namespace Mechanics.Enemies
{

    [AddComponentMenu("NPC/Behaviours/Patrol Point")]
    [DisallowMultipleComponent]
    public class PatrolPoint : OptimizedBehaviour
    {
        [SerializeField]
        [Min(0)]
        private float delayTime = 0f;

        public bool DelayAtPoint => delayTime > 0f;

        public float DelayTime
        {
            get => delayTime;
            set => delayTime = value;
        }
    }
}

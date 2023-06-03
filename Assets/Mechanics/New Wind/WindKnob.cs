using System;
using Avrahamy;
using BitStrap;
using UnityEngine;

namespace Mechanics.New_Wind
{
    [AddComponentMenu("Wind/Wind Knob")]
    public class WindKnob : OptimizedBehaviour
    {
        [SerializeField]
        [RequiredReference]
        private WindController myController;
        
        
        [Button("Set New Force")]
        private void OnValidate()
        {
            myController.Knob = this;
        }

        private void Awake()
        {
            myController.Knob = this;
        }


#if UNITY_EDITOR
        void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(0.13f, 0.93f, 1f, 1f);
            Gizmos.DrawSphere(transform.position, 0.3f);
        }
#endif

    }
}

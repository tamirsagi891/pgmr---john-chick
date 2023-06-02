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
        private void OnDrawGizmos()
        {
            Gizmos.DrawIcon(transform.position, name);
        }
#endif

    }
}

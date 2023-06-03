using System;
using System.Collections.Generic;
using Avrahamy.EditorGadgets;
using Avrahamy.Math;
using BitStrap;
using Elad.Scripts;
using UnityEditor;
using UnityEngine;
using Logger = Nemesh.Logger;

namespace Mechanics.New_Wind
{
    [AddComponentMenu("Wind/Wind Controller")]
    public class WindController : MonoBehaviour
    {
        [Serializable]
        public enum WindType
        {
            [InspectorName("Normal, Always on")]
            Regular,

            [InspectorName("Single Burst")]
            Explosive,

            [InspectorName("Glide only")]
            Glide,
        }

        [SerializeField]
        private WindType windType = WindType.Regular;

        // [SerializeField]
        // private bool useKnobForSize;

        [SerializeField]
        private bool useKnobForMagnitude;

        [ConditionalHide("useKnobForMagnitude", false, true)]
        [SerializeField]
        private float magnitude = 200f;

        [SerializeField]
        private float wantedDrag = 50f;

        [Space]
        [SerializeField]
        [Tooltip("Slowdown the particles by this factor")]
        private float particleForceFactor = 50f;

        [Space(2)]
        [Header("References")]
        [SerializeField]
        [RequiredReference]
        private WindEffectorController windEffectorController;

        public WindKnob Knob
        {
            get => _knob;
            set
            {
                _knob = value;
                _hasKnob = _knob != null;
                SetNewForce();
            }
        }

        public float Angle => Vector2.SignedAngle(Vector2.right, Force);

        public Vector2 Force
        {
            get
            {
                var force = Knob.transform.position - transform.position;
                return useKnobForMagnitude ? force : force.GetWithMagnitude(magnitude);
            }
        }

        public bool HasContact => windEffectorController.Contacts.Count > 0;

        public bool UseKnobForMagnitude
        {
            get => useKnobForMagnitude;
            set => useKnobForMagnitude = value;
        }

        public AreaEffector2D WindEffector => windEffectorController.WindEffector;

        public ParticleSystemForceField MyForceField => windEffectorController.MyForceField;

        public float WantedDrag
        {
            get => NotGlidingCondition ? 0 : wantedDrag;
            set => wantedDrag = value;
        }

        private bool NotGlidingCondition => windType == WindType.Glide && (!HasContact || !PlayerStatus.IsGliding);

        // public float Magnitude
        // {
        //     get
        //     {
        //         return useKnobForMagnitude ? Force.magnitude : magnitude;
        //     }
        // }

        private WindKnob _knob;
        private bool _hasKnob;

        private void OnValidate()
        {
            SetNewForce();
        }

        private void FixedUpdate()
        {
            SetNewForce();
        }

        [Button]
        public void SetNewForce()
        {
            // if (windType == WindType.Glide)
            // {
            //     return;
            // }

            if (!_hasKnob)
            {
                return;
            }

            var force = Force;

            MyForceField.directionX = force.x / particleForceFactor;
            MyForceField.directionY = force.y / particleForceFactor;

            if (NotGlidingCondition)
            {
                force = Vector2.zero;
            }

            WindEffector.forceAngle = Angle;
            WindEffector.forceMagnitude = force.magnitude;
            WindEffector.drag = WantedDrag;

            // if (useKnobForSize)
            // {
            //     var dist = Vector2.Distance(transform.position, Knob.transform.position);
            //     windEffectorController.transform.localScale = new Vector3(dist, dist, 1f);
            // }
        }


#if UNITY_EDITOR
        public void OnDrawGizmos()
        {
            Gizmos.DrawIcon(transform.position, "wind");
        }

        public void OnDrawGizmosSelected()
        {
            windEffectorController.OnDrawGizmosSelected();
            _knob!.OnDrawGizmosSelected();
        }
#endif

    }

}

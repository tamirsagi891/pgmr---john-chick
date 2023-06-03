using System;
using System.Collections.Generic;
using Avrahamy;
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
    public class WindController : OptimizedBehaviour
    {
        [Serializable]
        public enum WindType
        {
            [InspectorName("Constant")]
            Regular,
            
            [InspectorName("Constant Glide only")]
            Glide,

            [InspectorName("Single Burst")]
            Explosive,
            
            [InspectorName("Single Burst, Glide only")]
            ExplosiveGlide,
        }

        [SerializeField] // TODO: if we make this puclib, we must have setter that sets _firstExplosiveFrame!
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

        [Header("Explosive")]
        [SerializeField]
        private PassiveTimer explosiveTime = new(2f);

        [Space]
        [SerializeField]
        [Tooltip("Slowdown the particles by this factor")]
        private float particleForceFactor = 50f;

        [Space(2)]
        [Header("References")]
        [SerializeField]
        [RequiredReference]
        private WindEffectorController windEffectorController;
        
        [Space]
        [Header("Debug")]
        [SerializeField]
        protected bool debug;

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
                if (IsExplodingType && NotExplodingCondition)
                {
                    return Vector2.zero;
                }

                var force = Knob.transform.position - transform.position;
                if (transform.lossyScale.x < 0)
                {
                    force.x = -force.x;
                }
                
                var ret = UseKnobForMagnitude ? force : force.GetWithMagnitude(Magnitude);

                return ret;
            }
            set
            {
                Knob.transform.position = (Vector3)value + transform.position;
                // TODO: also update magnitude = force.magnitude?
                // TODO: Test scale?
            }
        }

        public bool UseKnobForMagnitude
        {
            get => useKnobForMagnitude;
            set => useKnobForMagnitude = value;
        }

        public bool HasContact => windEffectorController.Contacts.Count > 0;

        public AreaEffector2D WindEffector => windEffectorController.WindEffector;

        public ParticleSystemForceField MyForceField => windEffectorController.MyForceField;

        public float WantedDrag
        {
            get
            {
                return windType switch
                {
                    WindType.Glide => NotGlidingCondition ? 0 : wantedDrag,
                    WindType.Explosive => NotExplodingCondition ? 0 : wantedDrag,
                    WindType.ExplosiveGlide => NotGlidingCondition || NotExplodingCondition ? 0 : wantedDrag,
                    _ => wantedDrag
                };
            }
            set => wantedDrag = value;
        }

        public float Magnitude
        {
            get => magnitude;
            set => magnitude = value;
        }

        public Vector2 ForceOffset
        {
            get => _forceOffset;
            set
            {
                _forceOffset = value;
                _hasOffset = _forceOffset != Vector2.zero;
            }
        }

        private bool NotGlidingCondition => !HasContact || HasContact && !PlayerStatus.IsGliding;

        private bool NotExplodingCondition => !explosiveTime.IsSet || explosiveTime.IsSet && !explosiveTime.IsActive;

        private bool IsExplodingType => windType is WindType.Explosive or WindType.ExplosiveGlide;

        private bool IsGlidingType => windType is WindType.Glide or WindType.ExplosiveGlide;

        private Vector2 ForwardVector => transform.lossyScale.x < 0 ? Vector2.left : Vector2.right;

        private WindKnob _knob;
        private bool _hasKnob;
        private bool _firstExplosiveFrame;
        private bool _hasOffset;
        private Vector2 _forceOffset;

        #region MonoBehaviour

        private void OnValidate()
        {
            if (IsExplodingType && !explosiveTime.IsSet)
            {
                _firstExplosiveFrame = true;
            }

            SetNewForce();
        }

        private void Awake()
        {
            explosiveTime.Clear();
            if (IsExplodingType && !explosiveTime.IsSet)
            {
                _firstExplosiveFrame = true;
            }
        }


        private void FixedUpdate()
        {
            SetNewForce();
        }

        #endregion

        #region Public Methods

        [Button("Explode Wind")]
        public void Explode()
        {
            if (!IsExplodingType)
            {
                Logger.Log("Not Explosive type.", this);
                return;
            }

            _firstExplosiveFrame = false;
            explosiveTime.Start();
            SetNewForce();
            windEffectorController.ResumeParticles();
        }

        [Button]
        public void EndExplosion()
        {
            explosiveTime.Clear();
            windEffectorController.PauseParticles();
            _firstExplosiveFrame = false;
        }

        [Button]
        public void SetNewForce()
        {
            if (IsExplodingType)
            {
                if (NotExplodingCondition || _firstExplosiveFrame)
                {
                    EndExplosion();
                }
            }

            HandleBaseForce();
        }

        private void HandleBaseForce()
        {

            if (!_hasKnob)
            {
                return;
            }

            var force = Force;
            if (_hasOffset)
            {
                force = (ForceOffset + force).GetWithMagnitude(force.magnitude);
            }

            MyForceField.directionX = force.x / particleForceFactor;
            MyForceField.directionY = force.y / particleForceFactor;

            if (IsGlidingType && NotGlidingCondition)
            {
                force = Vector2.zero;
            }

            WindEffector.forceAngle = Angle;
            WindEffector.forceMagnitude = force.magnitude;
            WindEffector.drag = WantedDrag;
            if (debug)
            {
                // Logger.Log(PlayerStatus.JumpController && PlayerStatus.IsGliding);
            }

            // if (useKnobForSize)
            // {
            //     var dist = Vector2.Distance(transform.position, Knob.transform.position);
            //     windEffectorController.transform.localScale = new Vector3(dist, dist, 1f);
            // }
        }

        #endregion

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

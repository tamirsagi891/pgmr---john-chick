using System;
using Avrahamy.EditorGadgets;
using UnityEngine;
using Logger = Nemesh.Logger;

namespace Mechanics.New_Wind
{
    [AddComponentMenu("Wind/Wind Particle Offset")]
    [RequireComponent(typeof(WindController))]
    public class OffsetFieldByVelocity : MonoBehaviour
    {
        [SerializeField]
        private bool useRigidbody;
        
        [ConditionalHide("useRigidbody", true, true)]
        [SerializeField]
        private Vector2 offsetVector = new Vector2(-10f, 0f);
        
        [ConditionalHide("useRigidbody")]
        [SerializeField]
        private float offsetScale = 10f;
        
        private bool _hasRb;
        private Rigidbody2D _myRb;
        private WindController _controllerToReportTo;

        public bool UseRigidbody
        {
            get => useRigidbody;
            set
            {
                useRigidbody = value;
                if (!useRigidbody)
                {
                    _controllerToReportTo.ForceOffset = offsetVector;
                }
            }
        }

        private void OnValidate()
        {
            if (!useRigidbody && _controllerToReportTo != null)
            {
                _controllerToReportTo.ForceOffset = offsetVector;
            }
        }

        private void Awake()
        {
            _myRb = GetComponentInParent<Rigidbody2D>();
            _hasRb = _myRb != null;
            _controllerToReportTo = GetComponent<WindController>();
            if (!UseRigidbody)
            {
                _controllerToReportTo.ForceOffset = offsetVector;
            }
            // TODO: destroy if no rb
        }

        private void FixedUpdate()
        {
            if (!UseRigidbody || !_hasRb)
            {
                return;   
            }

            var offset = _myRb.velocity * offsetScale;
            if (transform.lossyScale.x > 0)
            {
                offset.x = -offset.x;
            }
            _controllerToReportTo.ForceOffset = offset;
        }

    }
}

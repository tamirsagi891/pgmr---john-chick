using System;
using Avrahamy;
using BitStrap;
using UnityEngine;
using UnityEngine.Serialization;
using Logger = Nemesh.Logger;

namespace Mechanics.New_Wind
{
    [AddComponentMenu("Wind/Wind Constraint")]
    public class WindConstraint : OptimizedBehaviour
    {
        [SerializeField]
        [TagSelector]
        private string gateTag = "Wind Gate";
        
        [SerializeField]
        private bool setKillImmediate = true;
        
        [FormerlySerializedAs("_windController")]
        [SerializeField]
        private WindController windController;
        private Transform _effectorControllerTransform;
        private Vector3 _originalPos;
        private Vector3 _originalScale;
        private Quaternion _originalRot;

        private void OnValidate()
        {
            if (windController == null)
            {
                windController = GetComponentInParent<WindController>();

            }
            // transform.parent.gameObject.TryGetComponent()  TODO: Switch to this
            _effectorControllerTransform = windController.EffectorController.transform;
        }

        private void Start()
        {
            if (windController == null)
            {
                windController = GetComponentInParent<WindController>();

            }
            // transform.parent.gameObject.TryGetComponent()  TODO: Switch to this
            _effectorControllerTransform = windController.EffectorController.transform;
            
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag(gateTag))
            {
                ApplyConstraint();
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            Logger.Log("Exit", other.gameObject);
            if (other.CompareTag(gateTag))
            {
                RemoveConstraint();
            }
        }

        [Button]
        private void ApplyConstraint()
        {
            if (setKillImmediate)
            {
                windController.EffectorController.SetKillImmediate(false);
            }
            
            _originalPos = _effectorControllerTransform.position;
            _originalScale = _effectorControllerTransform.localScale;
            _originalRot = _effectorControllerTransform.rotation;
            _effectorControllerTransform.position = transform.position;
            _effectorControllerTransform.localScale = transform.localScale;
            _effectorControllerTransform.rotation = transform.rotation;
        }
        
        [Button]
        private void RemoveConstraint()
        {
            if (setKillImmediate)
            {
                windController.EffectorController.SetKillImmediate(true);
            }
            
            _effectorControllerTransform.position = _originalPos;
            _effectorControllerTransform.localScale = _originalScale;
            _effectorControllerTransform.rotation = _originalRot;
        }


#if UNITY_EDITOR
        public void OnDrawGizmosSelected()
        {
            // Draw a semitransparent red cube at the transforms position
            Gizmos.color = new Color(0.73f, 0.68f, 0.07f);
            var transform1 = transform;
            Gizmos.DrawWireCube(transform1.position, transform1.lossyScale);
            if (windController != null)
            {
                windController.OnDrawGizmosSelected();
            }
        }
#endif
    }
}
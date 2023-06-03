using System.Collections.Generic;
using BitStrap;
using UnityEngine;

namespace Mechanics.New_Wind
{
    
    [AddComponentMenu("Wind/Utils/Effector Controller")]
    [RequireComponent(typeof(AreaEffector2D))]
    [RequireComponent(typeof(ParticleSystemForceField))]
    public class WindEffectorController : MonoBehaviour
    {
        [SerializeField]
        [TagSelector]
        private string playerTag = "Player";
        
        [Space]
        [SerializeField]
        [RequiredReference]
        private AreaEffector2D windEffector;  // TODO: get from code instead

        [SerializeField]
        [RequiredReference]
        private ParticleSystemForceField myForceField;

        public AreaEffector2D WindEffector
        {
            get => windEffector;
            set => windEffector = value;
        }

        public ParticleSystemForceField MyForceField
        {
            get => myForceField;
            set => myForceField = value;
        }

        public HashSet<GameObject> Contacts { get; } = new();

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag(playerTag))
            {
                Contacts.Add(other.gameObject);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag(playerTag))
            {
                Contacts.Remove(other.gameObject);
            }
        }
        
#if UNITY_EDITOR
        public void OnDrawGizmosSelected()
        {
            // Draw a semitransparent red cube at the transforms position
            Gizmos.color = new Color(0.13f, 0.93f, 1f, 0.21f);
            var transform1 = transform;
            Gizmos.DrawCube(transform1.position, transform1.lossyScale);
            var children = GetComponentsInChildren<WindParticleCallbackHandler>();
            foreach (var child in children)
            {
                child.OnDrawGizmosSelected();
            }
        }
#endif
    }
}

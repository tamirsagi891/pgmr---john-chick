using System.Collections.Generic;
using BitStrap;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.IMGUI.Controls;
#endif
using UnityEngine;
using Logger = Nemesh.Logger;

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
        private AreaEffector2D windEffector; // TODO: get from code instead

        [SerializeField]
        [RequiredReference]
        private ParticleSystemForceField myForceField;

        [SerializeField]
        private List<WindParticleCallbackHandler> myParticleSystems = new();

        [SerializeField]
        private BoxCollider2D myCollider;

        [SerializeField]
        private ParticleShape particleShape;

        [SerializeField]
        private Bounds myBounds;

        public BoxCollider2D MyCollider
        {
            get => myCollider;
            private set => myCollider = value;
        }

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

        public Bounds MyBounds => myBounds;

        #region Public Methods

        private void OnValidate()
        {
            SetBounds(myBounds);
        }

        public void SetBounds(Bounds value)
        {
            myBounds = value;
            particleShape.SetSize(myBounds.size);
            MyForceField.endRange = myBounds.size.x * 0.5f;
            MyForceField.length = MyBounds.size.y;
            MyCollider.size = myBounds.size;
        }

        public void SetKillImmediate(bool killImmediate)
        {
            foreach (var particle in myParticleSystems)
            {
                particle.KillImmediate = killImmediate;
            }
        }

        public void PauseParticles()
        {
            foreach (var particle in myParticleSystems)
            {
                particle.PauseParticles();
            }
        }

        public void ResumeParticles()
        {
            foreach (var particle in myParticleSystems)
            {
                particle.ResumeParticles();
            }
        }

        #endregion

        #region MonoBehaviour

        private void Awake()
        {
            // myParticleSystems.Clear();
            // myParticleSystems.AddRange(GetComponentsInChildren<WindParticleCallbackHandler>());
            // MyCollider = GetComponent<BoxCollider2D>();
        }

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

        #endregion

#if UNITY_EDITOR
        public void OnDrawGizmosSelected()
        {
            // Draw a semitransparent red cube at the transforms position
            Gizmos.color = new Color(0.13f, 0.93f, 1f, 0.1f);
            var transform1 = transform;
            Gizmos.DrawCube(transform1.position, myBounds.size);
            particleShape.OnDrawGizmosSelected();
        }
#endif
    }
}
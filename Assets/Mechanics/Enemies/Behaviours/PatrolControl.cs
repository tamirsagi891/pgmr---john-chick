using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Avrahamy.Collections;
using BitStrap;
using Cinemachine;
using UnityEngine;
using Logger = Nemesh.Logger;

namespace Mechanics.Enemies
{
    [AddComponentMenu("NPC/Behaviours/Patrol")]
    public class PatrolControl : MonoBehaviour, INpcMovementBehaviour
    {

        #region Inspector

        [HelpBox("The patrol points should be children of this object", HelpBoxAttribute.MessageType.Info)]
        [ReadOnly]
        [SerializeField]
        private List<PatrolPoint> patrolPoints;
        
        [SerializeField]
        [ReadOnly]
        private PatrolPoint target;

        [Space]
        [SerializeField]
        private float minDistanceToTarget = 0.3f;

        [Space]
        [SerializeField]
        private BaseNpc npcToReportTo;

        public PatrolPoint Target
        {
            get => target;
            set
            {
                // TODO: if old same has new - disable behaviour?
                target = value;
                _hasTarget = value != null;

                if (EnabledBehaviour)
                {
                    npcToReportTo.WalkTarget = _hasTarget ? target.transform : null;
                }
            }
        }

        #endregion

        #region Public Properties
        
        public bool EnabledBehaviour { get; set; }

        #endregion

        #region Private Fields

        private bool _hasTarget;
        
        #endregion

        #region MonoBehaviour

        

        private void Awake()
        {
            npcToReportTo.MovementBehaviour = this;
            // TODO: Control from BaseNpc
            EnabledBehaviour = true;

        }
        
        public void OnEnable()
        {
            ReloadPoints();
            npcToReportTo.events.onDeath.AddListener(Disable);
        }

        private void Start()
        {
            Target = target;
        }

        private void OnDisable()
        {
            Target = null;
            npcToReportTo.MovementBehaviour = null;
            npcToReportTo.events.onDeath.RemoveListener(Disable);
        }

        private void FixedUpdate()
        {
            if (_hasTarget && EnabledBehaviour)
            {
                if (Mathf.Abs(npcToReportTo.transform.position.x - Target.transform.position.x) < minDistanceToTarget) // TODO: optimize distance check
                {
                    GoToNextPoint();
                }
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns next target, for future proofing
        /// </summary>
        /// <returns></returns>
        [Button]
        public PatrolPoint ReloadPoints()
        {
            patrolPoints = new List<PatrolPoint>(GetComponentsInChildren<PatrolPoint>());

            if (patrolPoints.Count > 0)
            {
                if (Target == null || !patrolPoints.Contains(Target))
                {
                    Target = patrolPoints[0]; // TODO: reset path?

                    return Target;
                }
            }
            else
            {
                Target = null;
            }

            return Target;
        }

        [Button]
        public void GoToNextPoint()
        {
            GoToNextPoint(false);
        }
        
        public PatrolPoint GoToNextPoint(bool reverse)
        {
            if (patrolPoints.Count <= 0)
            {
                Target = null;
                return Target;
            }

            if (Target == null)
            {
                Target = patrolPoints[0]; // TODO: reset path?
                return Target;
            }

            var nextInd = patrolPoints.FindIndex(x => x == Target);
            nextInd = reverse ? patrolPoints.Count + nextInd - 1 : nextInd + 1;
            nextInd %= patrolPoints.Count;
            Target = patrolPoints[nextInd];

            return Target;
        }
        
        // TODO: GoToPoint(

        #endregion

        #region Private Methods

        private void Disable()
        {
            gameObject.SetActive(false);
        }
        
        #endregion

    }
}

using System.Collections.Generic;
using Avrahamy;
using BitStrap;
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

        [SerializeField]
        private bool checkBothAxis;
        
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

        public bool EnabledBehaviour
        {
            get => _enabledBehaviour;
            set
            {
                _enabledBehaviour = value;
            }
        }

        #endregion

        #region Private Fields

        private bool _hasTarget;

        private PassiveTimer _delayTimer = new();
        private bool _enabledBehaviour;

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
            // switch (_delayTimer.IsSet)
            // {
            //     case true when !_delayTimer.IsActive:
            //         _delayTimer.Clear();
            //         GoToNextPoint();
            //         return;
            //     case true:
            //         return;
            // }

            if (_hasTarget && EnabledBehaviour && CheckDistance())
            {
                if (target.DelayAtPoint)
                {
                    // _delayTimer.Start(target.DelayTime);
                    npcToReportTo.StopMovement(target.DelayTime); 
                    // TODO: inform that we are stopping at the point and let the npc decide instead.
                }
                // else
                // {
                GoToNextPoint();
                // }
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Returns next target, for future proofing
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
        public void GoToCurrentPoint()
        {
            if (Target == null)
            {
                GoToNextPoint();
            }
            else
            {
                Target = target;
            }
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

        private bool CheckDistance()
        {
            if (checkBothAxis)
            {
                var position = npcToReportTo.OffsetPosition.position;
                var positionTarget = Target.transform.position;
                return Vector2.Distance(position, positionTarget) <
                       minDistanceToTarget;
            }

            return Mathf.Abs(npcToReportTo.transform.position.x - Target.transform.position.x) <
                   minDistanceToTarget;
        }

        #endregion
    }
}
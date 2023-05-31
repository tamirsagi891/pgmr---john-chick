using System;
using System.Collections;
using Avrahamy;
using UnityEngine;
using UnityEngine.Events;

namespace Mechanics.Enemies
{
    [AddComponentMenu("NPC/Attack Behaviours/Dash And Alert")]
    [RequireComponent(typeof(Collider2D))]
    public class DashAndAlertControl : MonoBehaviour
    {
        #region Inspector

        [SerializeField]
        private bool useTrigger = true;

        [SerializeField]
        private PassiveTimer waitForDashTimer = new(1f);

        [SerializeField]
        private UnityEvent<BaseNpc> onAlert;

        [SerializeField]
        private UnityEvent<BaseNpc> onDash;

        [Space]
        [SerializeField]
        private BaseNpc npcToReportTo;

        #endregion

        #region Public Properties

        public float Radius
        {
            get => _radius;
            set
            {
                _radius = value;
                var col = _myCollider as CircleCollider2D;
                if (col != null)
                {
                    Radius = col.radius;
                }
            }
        }

        #endregion
        
        #region Private Fields

        private ICanBeAttacked _attackTarget;
        private Collider2D _myCollider;
        private float _radius;  // TODO: SerializeField

        #endregion

        #region Private Method

        private void Disable()
        {
            gameObject.SetActive(false);
        }

        #endregion

        #region MonoBehaviour

        private void Awake()
        {
            _myCollider = GetComponent<Collider2D>();
            var col = _myCollider as CircleCollider2D;
            if (col != null)
            {
                _radius = col.radius;
            }
        }

        private void OnEnable()
        {
            npcToReportTo.events.onDeath.AddListener(Disable);
        }

        private void OnDisable()
        {
            npcToReportTo.events.onDeath.RemoveListener(Disable);
        }

        private void Update()
        {
            if (waitForDashTimer.IsSet && !waitForDashTimer.IsActive)
            {
                waitForDashTimer.Clear();
                onDash.Invoke(npcToReportTo);
                npcToReportTo.StartMovement();
                npcToReportTo.Dash();
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!useTrigger)
            {
                return;
            }
            var attackTarget = other.GetComponent<ICanBeAttacked>();
            if (attackTarget != null && !waitForDashTimer.IsSet && !npcToReportTo.IsDashing)
            {
                StartDashAlertSequence();
            }
        }

        #endregion

        #region Public Methods

        public void StartDashAlertSequence()
        {
            waitForDashTimer.Start();
            npcToReportTo.StopMovement(waitForDashTimer.Duration);
            onAlert.Invoke(npcToReportTo);
        }

        #endregion
    }
}
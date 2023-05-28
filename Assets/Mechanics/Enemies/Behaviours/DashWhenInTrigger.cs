using System;
using System.Collections;
using Avrahamy;
using UnityEngine;
using UnityEngine.Events;

namespace Mechanics.Enemies
{
    [AddComponentMenu("NPC/Attack Behaviours/Dash When In Trigger")]
    [RequireComponent(typeof(Collider2D))]
    public class DashWhenInTrigger : MonoBehaviour
    {
        #region Inspector

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
        
        #region Private Fields

        private ICanBeAttacked _attackTarget;

        #endregion

        #region Private Method

        private void Disable()
        {
            gameObject.SetActive(false);
        }

        #endregion

        #region MonoBehaviour

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
            var attackTarget = other.GetComponent<ICanBeAttacked>();
            if (attackTarget != null && !waitForDashTimer.IsSet && !npcToReportTo.IsDashing)
            {
                waitForDashTimer.Start();
                npcToReportTo.StopMovement(waitForDashTimer.Duration);
                onAlert.Invoke(npcToReportTo);
            }
        }

        #endregion
    }
}
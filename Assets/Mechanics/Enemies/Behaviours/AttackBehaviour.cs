using UnityEngine;
using UnityEngine.Serialization;

namespace Mechanics.Enemies
{
    public class AttackBehaviour : MonoBehaviour
    {
        #region Inspector
        
        [Header("AttackBehaviour")]
        [FormerlySerializedAs("stopVelocityWhenEnterTrigger")]
        [SerializeField]
        protected bool stopVelocityWhenAttacking = true;

        [Space]
        [SerializeField]
        protected BaseNpc npcToReportTo;

        #endregion

        #region Public Properties

        public bool EnableBehaviour { get; set; } = true;

        #endregion
        
        #region Private Method

        protected virtual void Disable()
        {
            gameObject.SetActive(false);
        }

        #endregion

        #region MonoBehaviour

        protected virtual void OnEnable()
        {
            npcToReportTo.events.onDeath.AddListener(Disable);
        }

        protected virtual void OnDisable()
        {
            npcToReportTo.events.onDeath.RemoveListener(Disable);
        }
        
        #endregion
    }
}

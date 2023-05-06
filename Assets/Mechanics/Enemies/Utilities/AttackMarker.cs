using Avrahamy;
using UnityEngine;
using UnityEngine.Events;

namespace Mechanics.Enemies
{
    [AddComponentMenu("NPC/Attack Marker Example")]
    public class AttackMarker : MonoBehaviour
    {
        #region Inspector

        // TODO: get from BaseNpc, or from somewhere else. THIS IS TEMPORARY!
        [SerializeField]
        private PassiveTimer attackTimer = new(1f);

        [SerializeField]
        public UnityEvent onAttackStart;

        [SerializeField]
        public UnityEvent onAttackEnd;

        #endregion
        
        #region MonoBehaviour

        private void Update()
        {
            if (!attackTimer.IsSet || attackTimer.IsActive)
            {
                return;
            }

            onAttackEnd.Invoke();
            attackTimer.Clear();
        }

        #endregion

        #region Public Methods

        public void StartAttackMark()
        {
            attackTimer.Start();
            onAttackStart.Invoke();
        }

        #endregion
        
    }
}

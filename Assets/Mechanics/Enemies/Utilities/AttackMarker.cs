using Avrahamy;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Mechanics.Enemies
{
    [AddComponentMenu("NPC/Attack Marker Example")]
    public class AttackMarker : MonoBehaviour
    {
        #region Inspector

        // TODO: get from BaseNpc, or from somewhere else. THIS IS TEMPORARY!
        [FormerlySerializedAs("attackTimer")]
        [SerializeField]
        private PassiveTimer markerTimer = new(0.5f);

        [SerializeField]
        public UnityEvent onAttackStart;

        [SerializeField]
        public UnityEvent onAttackEnd;

        #endregion
        
        #region MonoBehaviour

        private void Update()
        {
            if (!markerTimer.IsSet || markerTimer.IsActive)
            {
                return;
            }

            onAttackEnd.Invoke();
            markerTimer.Clear();
        }

        #endregion

        #region Public Methods

        public void StartAttackMark()
        {
            markerTimer.Start();
            onAttackStart.Invoke();
        }

        #endregion
        
    }
}

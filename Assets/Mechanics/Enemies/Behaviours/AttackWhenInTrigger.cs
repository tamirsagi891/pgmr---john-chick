using UnityEngine;
using Logger = Nemesh.Logger;

namespace Mechanics.Enemies
{
    [AddComponentMenu("NPC/Attack Behaviours/Attack When In Trigger")]
    [RequireComponent(typeof(Collider2D))]
    public class AttackWhenInTrigger : MonoBehaviour // TODO: type
    {

        #region Inspector

        [SerializeField]
        private bool stopVelocityWhenEnterTrigger = true;

        [Space]
        [SerializeField]
        private BaseNpc npcToReportTo;

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

        private void Start() // TODO: make this also the attack strategy controller? or separate object?
        {
            _myCollider = GetComponent<Collider2D>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var attackTarget = other.GetComponent<ICanBeAttacked>();
            if (attackTarget != null)
            {
                AttackTarget = attackTarget;
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            var attackTarget = other.GetComponent<ICanBeAttacked>();
            if (attackTarget == AttackTarget)
            {
                AttackTarget = null;
            }
        }

        #endregion


        #region Private Fields

        private Collider2D _myCollider;

        private ICanBeAttacked _attackTarget;

        public ICanBeAttacked AttackTarget
        {
            get => _attackTarget;
            set
            {
                if (value == null)
                {
                    npcToReportTo.AttackTargets.Remove(_attackTarget);
                }
                else
                {
                    if (!npcToReportTo.AttackTargets.Contains(value))
                    {
                        npcToReportTo.AttackTargets.Add(value); // TODO: dont remove if someone else added?
                        if (stopVelocityWhenEnterTrigger)
                        {
                            npcToReportTo.StopMovement(Time.fixedDeltaTime);
                        }
                    }
                }
            
                _attackTarget = value;
            }
        }

        #endregion


    }
}

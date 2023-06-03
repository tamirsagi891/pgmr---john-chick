using Avrahamy;
using UnityEngine;
using UnityEngine.Serialization;
using Logger = Nemesh.Logger;

namespace Mechanics.Enemies
{
    [AddComponentMenu("NPC/Attack Behaviours/Attack When In Trigger")]
    [RequireComponent(typeof(Collider2D))]
    public class AttackWhenInTrigger : AttackBehaviour
    {

        #region MonoBehaviour

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
                    // TODO: move EnableBehaviour to encompass the entire setter?
                    if (EnableBehaviour && !npcToReportTo.AttackTargets.Contains(value))  
                    {
                        npcToReportTo.AttackTargets.Add(value); // TODO: dont remove if someone else added?
                        if (npcToReportTo.CanAttack && stopVelocityWhenAttacking)
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

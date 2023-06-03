using Avrahamy;
using Avrahamy.EditorGadgets;
using UnityEngine;
using Logger = Nemesh.Logger;

namespace Mechanics.Enemies
{

    [AddComponentMenu("NPC/Attack Behaviours/Attack Directly When In Trigger")]
    [RequireComponent(typeof(Collider2D))]
    public class AttackDirectlyOnContact : AttackBehaviour
    {

        #region Inspector

        [SerializeField]
        protected bool useNpcCooldown;

        [SerializeField]
        [BitStrap.ReadOnly(onlyInPlaymode = true)]
        [ConditionalHide("useNpcCooldown", true, true)]
        protected float contactAttackCooldown = 2f;
        
        protected readonly PassiveTimer AttackCooldown = new(5f);

        #endregion

        #region MonoBehaviour

        private void OnTriggerStay2D(Collider2D other)
        {
            if (AttackCooldown.IsSet && AttackCooldown.IsActive || !npcToReportTo.CanAttack)  // TODO: use the same cooldown as the attacker
            {
                return;
            }

            var attackTarget = other.GetComponent<ICanBeAttacked>();
            if (attackTarget != null)
            {
                var attackParams = npcToReportTo.GetAttackParameters();
                attackParams.Type = AttackType.Regular;
                npcToReportTo.HandleAttackStart(stopVelocityWhenAttacking);  // TODO: Reset attacker cooldown?
                npcToReportTo.events.onAttack.Invoke();
                attackTarget.Hurt(attackParams);
                AttackCooldown.Start(useNpcCooldown ? npcToReportTo.Cooldown : contactAttackCooldown);
            }
        }

        #endregion

    }
}

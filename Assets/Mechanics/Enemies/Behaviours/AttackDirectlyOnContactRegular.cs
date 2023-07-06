using UnityEngine;

namespace Mechanics.Enemies
{

    [AddComponentMenu("NPC/Attack Behaviours/Attack Directly When In Contact")]
    public class AttackDirectlyOnContactRegular : AttackDirectlyOnContact
    {
        #region MonoBehaviour

        private void OnCollisionStay2D(Collision2D other)
        {
            if (AttackCooldown.IsSet && AttackCooldown.IsActive || !npcToReportTo.CanAttack)  // TODO: use the same cooldown as the attacker
            {
                return;
            }

            var attackTarget = other.gameObject.GetComponent<ICanBeAttacked>();
            if (attackTarget != null)
            {
                var attackParams = npcToReportTo.GetAttackParameters();
                attackParams.Type = AttackType.Regular;
                if (useNpcCooldown)
                {
                    npcToReportTo.Attack(attackTarget, stopVelocityWhenAttacking);
                    return;
                }
                npcToReportTo.HandleAttackStart(stopVelocityWhenAttacking);  // TODO: Reset attacker cooldown?
                npcToReportTo.events.onAttack.Invoke();
                attackTarget.Hurt(attackParams);
                AttackCooldown.Start(useNpcCooldown ? npcToReportTo.Cooldown : contactAttackCooldown);
            }
        }
        
        #endregion

    }
}

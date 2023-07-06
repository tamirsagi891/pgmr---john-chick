using UnityEngine;

namespace Mechanics.Enemies
{
    [AddComponentMenu("NPC/Effectors/Change Attack On Trigger")]
    public class SetAttackTypeOnTrigger: BaseNpcEffector
    {
        [SerializeField]
        private AttackType attackType;

        [SerializeField]
        private float newCd = 7f;

        protected override void ApplyEffect(BaseNpc npc)
        {
            npc.MyStatsHandler.CurrentStats.type = attackType;
            npc.MyStatsHandler.CurrentStats.cooldown = newCd;
        }

        protected override void RemoveEffect(BaseNpc npc)
        {
            
        }
    }
}
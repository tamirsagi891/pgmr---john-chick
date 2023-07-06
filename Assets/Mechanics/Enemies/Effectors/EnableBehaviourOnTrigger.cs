using UnityEngine;

namespace Mechanics.Enemies
{
    [AddComponentMenu("NPC/Effectors/Enable Effect On Trigger")]
    public class EnableBehaviourOnTrigger: BaseNpcEffector
    {
        [SerializeField]
        private bool removeOnExit;
        
        [SerializeField]
        private BaseNpcEffector baseNpcEffector;
        
        protected override void ApplyEffect(BaseNpc npc)
        {
            baseNpcEffector.EffectActive = true;
        }

        protected override void RemoveEffect(BaseNpc npc)
        {
            if (removeOnExit)
            {
                baseNpcEffector.EffectActive = false;
            }
        }
    }
}
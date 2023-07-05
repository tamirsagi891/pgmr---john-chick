using UnityEngine;
using UnityEngine.Events;

namespace Mechanics.Enemies
{
    [AddComponentMenu("NPC/Effectors/Event On Trigger")]
    public class EventOnTrigger: BaseNpcEffector
    {
        [SerializeField]
        private UnityEvent onApply;

        protected override void ApplyEffect(BaseNpc npc)
        {
            onApply.Invoke();
        }

        protected override void RemoveEffect(BaseNpc npc)
        {

        }
    }
}
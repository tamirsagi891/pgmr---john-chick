using UnityEngine;

namespace Mechanics.Enemies
{
    [AddComponentMenu("NPC/Effectors/Switch Direction")]
    public class SwitchDirectionOnTrigger : BaseNpcEffector
    {
        // [Space]
        // [Header("Switch Direction")]
        // [SerializeField]
        // private bool jumpLeft = true;
        //
        // [SerializeField]
        // private bool jumpRight = true;

        protected override void ApplyEffect(BaseNpc npc)
        {
            npc.HandleDirectionSwitch();
        }

        protected override void RemoveEffect(BaseNpc npc)
        {
            // This effect doesn't need to be removed
        }
    }
}

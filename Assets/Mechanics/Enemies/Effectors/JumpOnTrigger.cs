using UnityEngine;

namespace Mechanics.Enemies
{
    [AddComponentMenu("NPC/Effectors/Jump In Trigger")]
    public class JumpOnTrigger : BaseNpcEffector
    {
        [Space]
        [Header("Jump In Trigger")]
        [SerializeField]
        private bool jumpLeft = true;

        [SerializeField]
        private bool jumpRight = true;

        protected override void ApplyEffect(BaseNpc npc)
        {
            switch (npc.CurrentDirection)
            {
                case Direction.Left when jumpLeft:
                case Direction.Right when jumpRight:
                    npc.Jump();
                    break;
            }
        }

        protected override void RemoveEffect(BaseNpc npc)
        {
            // This effect doesn't need to be removed
        }
    }
}

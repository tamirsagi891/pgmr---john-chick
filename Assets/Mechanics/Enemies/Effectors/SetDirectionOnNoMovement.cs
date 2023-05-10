using UnityEngine;

namespace Mechanics.Enemies
{
    [AddComponentMenu("NPC/Effectors/Set Direction On No Movement")]
    public class SetDirectionOnNoMovement : BaseNpcEffector
    {
        [Space]
        [Header("Set Direction")]
        [SerializeField]
        private Direction direction = Direction.Right;

        protected override void ApplyEffect(BaseNpc npc)
        {
            npc.SetDefaultDirection(direction);
        }

        protected override void RemoveEffect(BaseNpc npc)
        {
            npc.RemoveDefaultDirection();
        }
    }
}

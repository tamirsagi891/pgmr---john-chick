using UnityEngine;

namespace Mechanics.Enemies
{
    [AddComponentMenu("NPC/Effectors/Allow Edge Drop")]
    public class AllowEdgeDrop : BaseNpcEffector
    {
        [Header("Allow Edge Drop")]
        [SerializeField]
        private bool dropLeft = true;

        [SerializeField]
        private bool dropRight = true;

        protected override void ApplyEffect(BaseNpc npc)
        {
            switch (npc.CurrentDirection)
            {
                case Direction.Left when dropLeft:
                case Direction.Right when dropRight:
                    npc.DetectEdges = false;
                    break;
            }
        }

        protected override void RemoveEffect(BaseNpc npc)
        {
            npc.DetectEdges = true;
        }
    }
}

using System;
using BitStrap;
using UnityEngine;
using Logger = Nemesh.Logger;

namespace Mechanics.Enemies
{
    
    [AddComponentMenu("NPC/Effectors/Allow Edge Drop")]
    [RequireComponent(typeof(Collider2D))]
    public class AllowEdgeDrop : MonoBehaviour
    {

        #region Inspector

        [Header("Base NPC Effector")]
        [SerializeField]
        private bool effectActive = true;  // TODO: move to interface/base class

        [Space]
        [Header("Allow Edge Drop")]
        [SerializeField]
        private bool dropLeft = true;
        
        [SerializeField]
        private bool dropRight = true;

        #endregion

        #region MonoBehaviour

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!effectActive)
            {
                return;
            }
            var npc = other.GetComponentInParent<BaseNpc>();
            if (npc == null)
            {
                return;
            }

            switch (npc.CurrentDirection)
            {
                case Direction.Left when dropLeft:
                case Direction.Right when dropRight:
                    npc.DetectEdges = false;
                    break;
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!effectActive)
            {
                return;
            }
            var npc = other.GetComponentInParent<BaseNpc>();
            if (npc == null)
            {
                return;
            }

            npc.DetectEdges = true;
        }

        #endregion

    }
}

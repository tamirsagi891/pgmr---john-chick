using System;
using BitStrap;
using UnityEngine;
using Logger = Nemesh.Logger;

namespace Mechanics.Enemies
{
    
    [AddComponentMenu("NPC/Effectors/Jump In Trigger")]
    [RequireComponent(typeof(Collider2D))]
    public class JumpOnTrigger : MonoBehaviour
    {

        #region Inspector

        [Header("Base NPC Effector")]
        [SerializeField]
        private bool effectActive = true;  // TODO: move to interface/base class

        [Space]
        [Header("Jump On Trigger")]
        [SerializeField]
        private bool jumpLeft = true;
        
        [SerializeField]
        private bool jumpRight = true;

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
                case Direction.Left when jumpLeft:
                case Direction.Right when jumpRight:
                    npc.Jump();
                    break;
            }
        }

        #endregion

    }
}

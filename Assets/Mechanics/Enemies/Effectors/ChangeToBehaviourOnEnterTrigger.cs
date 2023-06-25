using System;
using BitStrap;
using UnityEngine;
using UnityEngine.Scripting;

namespace Mechanics.Enemies
{
    [AddComponentMenu("NPC/Effectors/Change To Behaviour")]
    public class ChangeToBehaviourOnEnterTrigger : BaseNpcEffector
    {
        [SerializeField]
        [RequireInterface(typeof(INpcMovementBehaviour))]
        private GameObject behaviourToUse;

        protected override void ApplyEffect(BaseNpc npc)
        {
            throw new System.NotImplementedException();
        }

        protected override void RemoveEffect(BaseNpc npc)
        {
            throw new System.NotImplementedException();
        }
    }
}
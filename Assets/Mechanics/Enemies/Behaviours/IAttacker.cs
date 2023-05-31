using System;
using UnityEngine;

namespace Mechanics.Enemies
{
    public interface IAttacker
    {
        public bool Attack(ICanBeAttacked attackTarget);

        public void DropPickup(ICanBeAttacked attackTarget)
        {
            return;
        }

        public float GetDamage()
        {
            return GetAttackParameters().Damage;
        }

        public Vector2 GetKnockBack()
        {
            return GetAttackParameters().KnockBack;
        }

        public AttackParameters GetAttackParameters();
    }

    [Serializable]
    public class AttackParameters
    {
        public AttackParameters(IAttacker attacker,
            float damage = 0f,
            Transform followTransform = null, 
            AttackType type = AttackType.Regular, 
            Vector2 knockBack = new ()
            )
        {
            Attacker = attacker;
            FollowTransform = followTransform;
            Type = type;
            KnockBack = knockBack;
            Damage = damage;
        }

        public float Damage { get; }
        public Vector2 KnockBack { get; }
        public AttackType Type { get; }
        public Transform FollowTransform { get; }
        public IAttacker Attacker { get; }
    }

    public enum AttackType
    {
        Regular,
        Pickup
    }
}

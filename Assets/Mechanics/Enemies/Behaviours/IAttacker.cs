using System;
using UnityEngine;

namespace Mechanics.Enemies
{
    public interface IAttacker
    {
        public bool Attack(ICanBeAttacked attackTarget);

        public AttackParameters GetAttackParameters();

        public void DropPickup()
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

    }

    [Serializable]
    public class AttackParameters
    {
        public AttackParameters(IAttacker attacker,
            float damage = 0f,
            Transform followTransform = null, 
            AttackType type = AttackType.Regular, 
            Vector2 knockBack = new (),
            float shotSpeed = 0f,
            Direction direction = Direction.Right
        )
        {
            Attacker = attacker;
            Direction = direction;
            FollowTransform = followTransform;
            Type = type;
            KnockBack = knockBack;
            Damage = damage;
            ShotSpeed = shotSpeed;
        }

        public float Damage { get; set; }
        public Vector2 KnockBack { get; set; }
        public AttackType Type { get; set; }
        public Transform FollowTransform { get; set; }
        public IAttacker Attacker { get; set; }

        public float ShotSpeed { get; set; }

        public Direction Direction { get; set; }

        public override string ToString()
        {
            return $"Attacker: {Attacker}.  Type: {Type} | DMG {Damage} | KB {KnockBack}";
        }
    }

    public enum AttackType
    {
        Shot,
        Regular,
        Pickup
    }
}

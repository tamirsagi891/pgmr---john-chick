using System;
using Elad.Scripts.Combat;
using UnityEngine;
using Logger = Nemesh.Logger;

namespace Mechanics.Enemies
{
    [AddComponentMenu("Player/Player Attack Controller")]
    public class PlayerAttackController : MonoBehaviour, IAttacker
    {
        [SerializeField] private int attackAmount = 10;
        [SerializeField] private Vector2 knockBack = Vector2.zero;
        private Damageable _myDamageable;

        [SerializeField] private bool debug;

        private void Awake()
        {
            _myDamageable = GetComponentInParent<Damageable>();
        }

        private void Start()
        {
            
        }

        public bool Attack(ICanBeAttacked attackTarget)
        {
            var attackParameters = GetAttackParameters();
            bool gotHit = attackTarget.Hurt(attackParameters);
            Logger.Log($"Attacking {attackTarget} ({attackParameters.Type}) for {attackParameters.Damage}  kb {attackParameters.KnockBack}", this);
            return gotHit;
        }

        public float GetDamage()
        {
            return attackAmount;
        }

        public Vector2 GetKnockBack()
        {
            if (transform.parent)
            {
                return transform.parent.localScale.x > 0 ? knockBack : new Vector2(-knockBack.x, knockBack.y);    
            }

            var retValue = new Vector2(0f, 0f);
            return retValue;
        }

        public AttackParameters GetAttackParameters()
        {
            // TODO: if attack is feather or egg - we want different?
            return new AttackParameters(
                attacker: this,
                damage: attackAmount,
                knockBack: knockBack,
                type: AttackType.Regular,
                followTransform: _myDamageable.transform);
        }
    }
}
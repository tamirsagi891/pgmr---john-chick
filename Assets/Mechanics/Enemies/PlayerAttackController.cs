using UnityEngine;
using Logger = Nemesh.Logger;

namespace Mechanics.Enemies
{
    [AddComponentMenu("Player/Player Attack Controller")]
    public class PlayerAttackController : MonoBehaviour, IAttacker, ICanBeAttacked
    {
        [SerializeField]
        private int attackAmount = 10;

        [SerializeField]
        private Vector2 knockBack = Vector2.zero;

        private Damageable _myDamageable;

        [SerializeField]
        private bool debug;

        private void Awake()
        {
            _myDamageable = GetComponentInParent<Damageable>();
        }

        public bool Attack(ICanBeAttacked attackTarget)
        {
            bool gotHit = attackTarget.Hurt(this);
            if (gotHit)
            {
                Logger.Log($"Attacking {attackTarget} for {GetDamage()}  kb {GetKnockBack()}", this);
            }

            return gotHit;
        }

        public float GetDamage()
        {
            return attackAmount;
        }

        public Vector2 GetKnockBack()
        {
            return transform.parent.localScale.x > 0 ? knockBack : new Vector2(-knockBack.x, knockBack.y);
        }

        public bool Hurt(IAttacker attacker)
        {
            Logger.Log($"Attacked by {attacker} for {attacker.GetDamage()}  kb {attacker.GetKnockBack()}", this);

            return _myDamageable.Hit((int)attacker.GetDamage(), attacker.GetKnockBack());
        }
    }
}
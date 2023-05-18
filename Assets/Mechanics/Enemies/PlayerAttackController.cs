using UnityEngine;
using Logger = Nemesh.Logger;

namespace Mechanics.Enemies
{
    [AddComponentMenu("Player/Player Attack Controller")]
    public class PlayerAttackController : MonoBehaviour, IAttacker, ICanBeAttacked
    {
        [SerializeField] private int attackAmount = 10;
        [SerializeField] private Vector2 knockBack = Vector2.zero;
        private Damageable _myDamageable;

        [SerializeField] private bool debug;

        private void Awake()
        {
            _myDamageable = GetComponentInParent<Damageable>();
        }

        public bool Attack(ICanBeAttacked attackTarget)
        {
            bool gotHit = attackTarget.Hurt(this);
            Logger.Log($"Attacking {attackTarget} for {GetDamage()}  kb {GetKnockBack()}", this);
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

        public bool Hurt(IAttacker attacker)
        {
            Logger.Log($"Attacked by {attacker} for {attacker.GetDamage()}  kb {attacker.GetKnockBack()}", this);

            return _myDamageable.GotHit((int) attacker.GetDamage(), attacker.GetKnockBack());
        }
    }
}
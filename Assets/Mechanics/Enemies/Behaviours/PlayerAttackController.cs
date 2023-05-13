using UnityEngine;
using Logger = Nemesh.Logger;

namespace Mechanics.Enemies
{
    [AddComponentMenu("Player/Player Attack Controller")]
    public class PlayerAttackController : MonoBehaviour, IAttacker, ICanBeAttacked
    {
        [SerializeField]
        private bool debug;
        
        public bool Attack(ICanBeAttacked attackTarget)
        {
            Logger.Log($"Attacking {attackTarget}", this);
            attackTarget.Hurt(this);
            return true;
        }

        public float GetDamage()
        {
            return 0;
        }

        public bool Hurt(IAttacker attacker)
        {
            Logger.Log($"Attacked By {attacker} for {attacker.GetDamage()} dmg", this);
            return true;
        }
    }
}

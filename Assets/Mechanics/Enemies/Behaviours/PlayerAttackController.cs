using UnityEngine;

namespace Mechanics.Enemies
{
    [AddComponentMenu("Player/Player Attack Controller")]
    public class PlayerAttackController : MonoBehaviour, IAttacker, ICanBeAttacked
    {
        public bool Attack(ICanBeAttacked attackTarget)
        {
            return true;
        }

        public float GetDamage()
        {
            return 0;
        }

        public bool Hurt(IAttacker attacker)
        {
            return true;
        }
    }
}

using UnityEngine;
using Logger = Nemesh.Logger;

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
            Logger.Log("Attacked by", attacker as MonoBehaviour);
            return true;
        }
    }
}

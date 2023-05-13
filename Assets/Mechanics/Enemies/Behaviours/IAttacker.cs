using UnityEngine;

namespace Mechanics.Enemies
{
    public interface IAttacker
    {
        public bool Attack(ICanBeAttacked attackTarget);

        public float GetDamage();

        public Vector2 GetKnockBack()
        {
            return Vector2.zero;
        }
    }
}

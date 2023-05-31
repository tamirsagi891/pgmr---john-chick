using UnityEngine;

namespace Mechanics.Enemies
{
    public interface ICanBeAttacked
    {
        public bool Hurt(AttackParameters attackParameters);

        public Transform GetTransform();
    }
}

namespace Mechanics.Enemies
{
    public interface IAttacker
    {
        public bool Attack(ICanBeAttacked attackTarget);

        public float GetDamage();
    }
}

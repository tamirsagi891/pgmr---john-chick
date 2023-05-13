namespace Mechanics.Enemies
{
    public interface ICanBeAttacked
    {
        public bool Hurt(IAttacker attacker);
    }
}

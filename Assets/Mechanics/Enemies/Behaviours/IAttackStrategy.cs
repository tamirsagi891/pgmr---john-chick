namespace Mechanics.Enemies
{
    public interface IAttackStrategy // TODO: Created by ChatGPT
    {
        public bool IsAttacking { get; }
        public bool Attack();
        public void UpdateStrategy();
        public void SetTarget(PlayerAttackController target);
    }
}

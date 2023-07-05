namespace Mechanics.Enemies
{
    public interface INpcMovementBehaviour
    {
        public bool EnabledBehaviour { get; set; }

        public void GoToNextPoint();
        public void GoToCurrentPoint();
    }
}

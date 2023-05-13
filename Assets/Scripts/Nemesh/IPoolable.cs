namespace Nemesh
{
    public interface IPoolable
    {
        void ReleaseSelf();
        void InitObject();
    }
}

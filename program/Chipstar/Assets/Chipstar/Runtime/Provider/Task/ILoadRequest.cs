namespace Chipstar.Downloads
{
    public interface ILoadJob : ILoadTask
    {
        void Update();
        void Done();
    }
}
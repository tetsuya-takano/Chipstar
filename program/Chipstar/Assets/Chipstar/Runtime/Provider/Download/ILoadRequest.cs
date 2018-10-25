namespace Chipstar.Downloads
{
    internal interface ILoadRequest
    {
        bool IsCompleted { get; }

        void Update();
        void Done();
    }
}
using System;

namespace Chipstar.Downloads
{
    public interface ILoadTask : IDisposable
    {
        float       Progress    { get; }
        bool        IsCompleted { get; }
    }

    public interface ILoadTask<T> : ILoadTask
    {
        T Content { get; }
    }
}

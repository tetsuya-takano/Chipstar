using System;

namespace Chipstar.Downloads
{
    public interface IAssetLoadTask<T> : ILoadTask
    {
        Action<T>   OnLoaded    { set; }
    }
    public interface ISceneLoadTask : ILoadTask
    {
        Action OnLoaded { set; }
    }
    public interface ILoadTask : IDisposable
    {
        float   Progress    { get; }
        bool    IsCompleted { get; }
    }
}
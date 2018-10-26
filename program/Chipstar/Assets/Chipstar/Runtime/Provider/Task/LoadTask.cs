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
    public interface ILoadRequest : ILoadTask
    {
        Action OnCompleted { get; }
    }

    public interface ILoadTask : IDisposable
    {
        float   Progress    { get; }
        bool    IsCompleted { get; }
    }


    public abstract class LoadTask<T>
                : ILoadTask
        where T : ILoadTask
    {
        public abstract bool  IsCompleted   { get; }
        public abstract float Progress      { get; }

        /// <summary>
        /// 破棄処理
        /// </summary>
        public void Dispose()
        {

        }
    }
}

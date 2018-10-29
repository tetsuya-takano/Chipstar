using UnityEngine;
using System;

namespace Chipstar.Downloads
{
    public interface IJobCreator : IDisposable
    {
        ILoadJob<byte[]>            CreateBytesLoad   ( IJobEngine engine, IAccessLocation location );
        ILoadJob<string>            CreateTextLoad    ( IJobEngine engine, IAccessLocation location );
        ILoadJob<AssetBundle>       CreateBundleFile  ( IJobEngine engine, IAccessLocation location );
        ILoadJob<UnityEngine.Object>CreateAssetLoad   ( IJobEngine engine, IAccessLocation location );
    }
    public class JobCreator : IJobCreator
    {
        //=======================================
        //  変数
        //=======================================
        protected Func<IAccessLocation, ILoadJob<string>>             OnTextLoad     { get; set; }
        protected Func<IAccessLocation, ILoadJob<byte[]>>             OnBytesLoad    { get; set; }
        protected Func<IAccessLocation, ILoadJob<AssetBundle>>        OnBundleLoad   { get; set; }
        protected Func<IAccessLocation, ILoadJob<UnityEngine.Object>> OnAssetLoad    { get; set; }

        //=======================================
        //  関数
        //=======================================
        public JobCreator(
            Func<IAccessLocation, ILoadJob<byte[]>>             onBytesLoad,
            Func<IAccessLocation, ILoadJob<string>>             onTextLoad,
            Func<IAccessLocation, ILoadJob<AssetBundle>>        onBundleLoad,
            Func<IAccessLocation, ILoadJob<UnityEngine.Object>> onAssetLoad 
        )
        {
            OnTextLoad   = onTextLoad;
            OnBundleLoad = onBundleLoad;
            OnAssetLoad  = onAssetLoad;
            OnBytesLoad  = onBytesLoad;
        }

        public void Dispose()
        {
            OnTextLoad  = null;
            OnBundleLoad= null;
            OnAssetLoad = null;
            OnBytesLoad = null;
        }
        /// <summary>
        /// テキスト取得リクエスト
        /// </summary>
        public ILoadJob<byte[]> CreateBytesLoad( IJobEngine engine, IAccessLocation location )
        {
            return AddJob( engine, OnBytesLoad( location ) );
        }

        /// <summary>
        /// テキスト取得リクエスト
        /// </summary>
        public ILoadJob<string> CreateTextLoad( IJobEngine engine, IAccessLocation location )
        {
            return AddJob( engine, OnTextLoad( location ) );
        }

        public ILoadJob<AssetBundle> CreateBundleFile( IJobEngine engine, IAccessLocation location )
        {
            return AddJob( engine, OnBundleLoad( location ) );
        }

        public ILoadJob<UnityEngine.Object> CreateAssetLoad( IJobEngine engine, IAccessLocation location )
        {
            return AddJob( engine, OnAssetLoad( location ) );
        }

        protected virtual ILoadJob<T> AddJob<T>( IJobEngine engine, ILoadJob<T> job )
        {
            engine.Enqueue( job );
            return job;
        }
    }
}
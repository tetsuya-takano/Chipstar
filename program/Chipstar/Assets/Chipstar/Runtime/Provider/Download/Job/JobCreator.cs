using UnityEngine;
using System;

namespace Chipstar.Downloads
{
    public interface IJobCreator<TRuntimeData> : IDisposable
        where TRuntimeData : IRuntimeBundleData<TRuntimeData>
    {
        ILoadJob<byte[]>            CreateBytesLoad     ( IJobEngine engine, IAccessLocation location );
        ILoadJob<string>            CreateTextLoad      ( IJobEngine engine, IAccessLocation location );
        ILoadJob<AssetBundle>       CreateBundleFile    ( IJobEngine engine, IAccessLocation location );
        ILoadJob<T>                 CreateAssetLoad<T>  ( IJobEngine engine, AssetData<TRuntimeData> data ) where T : UnityEngine.Object;
    }
    public abstract class JobCreator<TRuntimeData> : IJobCreator<TRuntimeData>
        where TRuntimeData : IRuntimeBundleData<TRuntimeData>
    {
        //=======================================
        //  変数
        //=======================================

        //=======================================
        //  関数
        //=======================================

        /// <summary>
        /// 破棄処理
        /// </summary>
        public void Dispose()
        {
            DoDispose();
        }
        protected virtual void DoDispose() { }

        /// <summary>
        /// 生データ取得リクエスト
        /// </summary>
        public ILoadJob<byte[]> CreateBytesLoad( IJobEngine engine, IAccessLocation location )
        {
            return AddJob( engine, DoCreateBytesLoad( location ) );
        }
        /// <summary>
        /// テキスト取得リクエスト
        /// </summary>
        public ILoadJob<string> CreateTextLoad( IJobEngine engine, IAccessLocation location )
        {
            return AddJob( engine, DoCreateTextLoad( location ) );
        }
        public ILoadJob<AssetBundle> CreateBundleFile( IJobEngine engine, IAccessLocation location )
        {
            return AddJob( engine, DoCreateBundleLoad( location ) );
        }
        public ILoadJob<T> CreateAssetLoad<T>( IJobEngine engine, AssetData<TRuntimeData> assetData ) 
            where T : UnityEngine.Object
        {
            return AddJob( engine, DoCreateAssetLoad<T>( assetData ) );
        }
        protected virtual ILoadJob<T> AddJob<T>( IJobEngine engine, ILoadJob<T> job )
        {
            engine.Enqueue( job );
            return job;
        }

        protected abstract ILoadJob<byte[]>         DoCreateBytesLoad   ( IAccessLocation location );
        protected abstract ILoadJob<string>         DoCreateTextLoad    ( IAccessLocation location );
        protected abstract ILoadJob<AssetBundle>    DoCreateBundleLoad  ( IAccessLocation location );
        protected abstract ILoadJob<T>              DoCreateAssetLoad<T>( AssetData<TRuntimeData> location ) where T : UnityEngine.Object;
    }
}
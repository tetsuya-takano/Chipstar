﻿using UnityEngine;
using System;

namespace Chipstar.Downloads
{
    public interface IJobCreator<TRuntimeData> : IDisposable
        where TRuntimeData : IRuntimeBundleData<TRuntimeData>
    {
        ILoadJob<byte[]>        BytesLoad       ( IJobEngine engine, IAccessLocation location );
        ILoadJob<string>        TextLoad        ( IJobEngine engine, IAccessLocation location );
        ILoadJob<AssetBundle>   DownloadBundle  ( IJobEngine engine, IAccessLocation location );
		ILoadJob<AssetBundle>	OpenLocalBundle	( IJobEngine engine, IAccessLocation location );
		ILoadJob<T>             AssetLoad<T>    ( IJobEngine engine, AssetData<TRuntimeData> data ) where T : UnityEngine.Object;
    }
    public abstract class JobCreator<TRuntimeData> 
		:	IJobCreator<TRuntimeData>
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
        public ILoadJob<byte[]> BytesLoad( IJobEngine engine, IAccessLocation location )
        {
            return AddJob( engine, DoCreateBytesLoad( location ) );
        }
        /// <summary>
        /// テキスト取得リクエスト
        /// </summary>
        public ILoadJob<string> TextLoad( IJobEngine engine, IAccessLocation location )
        {
            return AddJob( engine, DoCreateTextLoad( location ) );
        }
		/// <summary>
		/// アセットバンドルDL
		/// </summary>
        public ILoadJob<AssetBundle> DownloadBundle( IJobEngine engine, IAccessLocation location )
        {
            return AddJob( engine, DoCreateDownload( location ) );
        }
		/// <summary>
		/// ローカルアセットバンドルオープン
		/// </summary>
		public ILoadJob<AssetBundle> OpenLocalBundle( IJobEngine engine, IAccessLocation location )
		{
			return AddJob( engine, DoCreateLocalLoad( location ));
		}

		public ILoadJob<T> AssetLoad<T>( IJobEngine engine, AssetData<TRuntimeData> assetData ) 
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
        protected abstract ILoadJob<AssetBundle>    DoCreateDownload	( IAccessLocation location );
		protected abstract ILoadJob<AssetBundle>    DoCreateLocalLoad	( IAccessLocation location );
        protected abstract ILoadJob<T>              DoCreateAssetLoad<T>( AssetData<TRuntimeData> location ) where T : UnityEngine.Object;
	}
}
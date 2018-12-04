﻿using System;
using System.Collections;
using UnityEngine;

namespace Chipstar.Downloads
{
    public interface IAssetBundleLoadProvider : IDisposable
    {
        IEnumerator	InitLoad    ( );
		ILoadResult	Load		( string path );

		void DoUpdate();
	}

	/// <summary>
	/// 読み込みまわりの管理
	/// ダウンロード / キャッシュロード付近
	/// </summary>
	public class AssetBundleLoadProvider<TRuntimeData> 
                                : IAssetBundleLoadProvider
            where TRuntimeData  : IRuntimeBundleData<TRuntimeData>

    {
        //===============================
        //  プロパティ
        //===============================
        private IStorageDatabase            StorageDatabase { get; set; } // ローカルストレージのキャッシュ情報
        private ILoadDatabase<TRuntimeData> LoadDatabase    { get; set; } // コンテンツテーブルから作成したDB
        private IJobEngine                  JobEngine       { get; set; } // DLエンジン
        private IJobCreator					JobCreator      { get; set; } // ジョブの作成

        //===============================
        //  関数
        //===============================

        public AssetBundleLoadProvider
            ( 
                ILoadDatabase<TRuntimeData> loadDatabase,
				IStorageDatabase			storageDatabase,
                IJobEngine                  dlEngine,
                IJobCreator					jobCreator
            )
        {
            LoadDatabase    = loadDatabase;
			StorageDatabase = storageDatabase;
            JobEngine       = dlEngine;
            JobCreator      = jobCreator;

        }

		public void Dispose()
		{
			JobCreator	.Dispose();
			JobEngine	.Dispose();

			StorageDatabase	= null;
			LoadDatabase	= null;
			JobCreator      = null;
			JobEngine       = null;
		}

		/// <summary>
		/// 初期化処理
		/// </summary>
		public IEnumerator InitLoad( )
        {
			Chipstar.Log_Downloader_StartInit( );
			//	コンテンツデータの取得
			var location     = LoadDatabase.ToBuildMapLocation( );
			var loadBuildMap = DoInitielizeLoad( location );
			Chipstar.Log_Downloader_RequestBuildMap( location );
			while( !loadBuildMap.IsCompleted )
            {
                yield return null;
            }
			yield return LoadDatabase	.Initialize( loadBuildMap.Content );
			yield return StorageDatabase.Initialize( );

            yield break;
        }

		/// <summary>
		/// ロード処理
		/// </summary>
		public ILoadResult Load( string path )
		{
			Chipstar.Log_LoadStart( path );
			if( !LoadDatabase.Contains( path ) )
			{
				Chipstar.Log_AssetNotFound( path );
				return LoadSkip.Default;
			}
			var data = LoadDatabase.GetAssetData( path );
			return DoLoad( data );
		}

		/// <summary>
		/// 更新処理
		/// </summary>
		public void DoUpdate()
		{
			JobEngine.Update();
		}

		/// <summary>
		/// 初期化ロード処理
		/// </summary>
		protected virtual ILoadJob<byte[]> DoInitielizeLoad( IAccessLocation location )
		{
			return JobCreator.BytesLoad( JobEngine, location );
		}

		/// <summary>
		/// アセットを取得するため必要なデータを一通りロード
		/// </summary>
		protected virtual ILoadResult DoLoad( AssetData<TRuntimeData> data )
        {
            var bundleData = data.BundleData;
			//	対象のロード
			var	preloadJob = DoDownloadWithNeedAll( bundleData );
            //  ロード
            return preloadJob;
        }

        /// <summary>
        /// 依存アセットバンドルを一通りロード
        /// </summary>
        protected virtual ILoadResult DoDownloadWithNeedAll( TRuntimeData data )
        {
            //  依存先がないなら自分だけ
            if (data.Dependencies.Length == 0)
            {
                return DoLoadCore( data );
            }
            return DoDownloadDependencies( data );
        }
        /// <summary>
        /// 事前ロードのみ
        /// </summary>
        protected virtual ILoadResult DoDownloadDependencies( TRuntimeData data )
        {
            //  事前に必要な分を合成
            var dependencies= data.Dependencies;
            var preloadJob  = new ILoadResult[ dependencies.Length ];
            for( int i = 0; i < preloadJob.Length; i++ )
            {
				var tmp = dependencies[i];
				preloadJob[i] = DoLoadCore( tmp );
            }
			return preloadJob
				.ToParallel()
				.ToJoin( DoLoadCore( data ) );
        }

		/// <summary>
		/// ロード処理
		/// </summary>
		protected virtual ILoadResult DoLoadCore( TRuntimeData data )
		{
			if( data.IsOnMemory )
			{
				//	ロードしてあるならしない
				return LoadSkip.Default;
			}
			if( StorageDatabase.HasStorage( data ) )
			{
				//	更新がないならそのままローカルのファイルを開く
				return DoLocalOpen( data );
			}
			///	DLしてから開く
			return DoLoadNewFile( data )
					.ToJoin( DoLocalOpen( data ) );
		}

		/// <summary>
		/// ダウンロード
		/// </summary>
		protected virtual ILoadResult DoLoadNewFile( TRuntimeData data )
		{
            var location    = LoadDatabase.ToBundleLocation( data );
            var job         = JobCreator.BytesLoad( JobEngine, location );
			return new LoadResult<byte[]>(
				job,
				onCompleted: ( content ) =>
				{
					//	ファイルのDL → 書き込み → ローカルロード
					//	剥がしたい
					StorageDatabase.Write( data, content );
					StorageDatabase.Apply();
				}
			);
		}

		/// <summary>
		/// ローカルファイルを開く
		/// </summary>
		protected virtual ILoadResult DoLocalOpen( TRuntimeData data )
		{
			var location	= StorageDatabase.ToLocation( data.Name );
			var job			= JobCreator.OpenLocalBundle( JobEngine, location );
			return new LoadResult<AssetBundle>(
				job,
				onCompleted: ( content ) =>
				{
					data.OnMemory( content );
				}
			);
		}

	}
}
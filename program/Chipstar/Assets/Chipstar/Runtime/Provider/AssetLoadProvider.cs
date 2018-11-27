using System;
using System.Collections;
using UnityEngine;

namespace Chipstar.Downloads
{
    //==============================
    //  読み込み統括
    //  リクエストを受け取って、タスクを返す
    //  使用者が触るのはこれ
    //==============================


    public interface IAssetLoadProvider
    {
        IEnumerator				InitLoad    ( string dbFile, string localVersion );
		ILoadResult				Load		( string path );

		void DoUpdate();
	}

	/// <summary>
	/// 読み込みマネージャ
	/// </summary>
	public class AssetLoadProvider<TRuntimeData> 
                                : IAssetLoadProvider
            where TRuntimeData  : IRuntimeBundleData<TRuntimeData>

    {
        //===============================
        //  プロパティ
        //===============================
        private ICacheDatabase                  CacheDatabase   { get; set; } // ローカルストレージのキャッシュ情報
        private ILoadDatabase<TRuntimeData>     LoadDatabase    { get; set; } // コンテンツテーブルから作成したDB
        private IJobEngine                      JobEngine       { get; set; } // DLエンジン
        private IJobCreator<TRuntimeData>       JobCreator      { get; set; } // ジョブの作成

        //===============================
        //  関数
        //===============================

        public AssetLoadProvider
            ( 
                ILoadDatabase<TRuntimeData> loadDatabase,
				ICacheDatabase				cacheDatabase,
                IJobEngine                  dlEngine,
                IJobCreator<TRuntimeData>   jobCreator
            )
        {
            LoadDatabase    = loadDatabase;
			CacheDatabase   = cacheDatabase;
            JobEngine       = dlEngine;
            JobCreator      = jobCreator;
        }

		/// <summary>
		/// 初期化処理
		/// </summary>
        public IEnumerator InitLoad( string fileName, string localVersionFile )
        {
			//	コンテンツデータの取得
			var location     = LoadDatabase.ToBuildMapLocation( fileName );
            var loadBuildMap = DoInitielizeLoad( location );
            while( !loadBuildMap.IsCompleted )
            {
                yield return null;
            }

            LoadDatabase.Initialize( loadBuildMap.Content );

			yield return CacheDatabase.Initialize( localVersionFile );

            yield break;
        }

		/// <summary>
		/// ロード処理
		/// </summary>
		public ILoadResult Load( string path )
		{
			var data = LoadDatabase.Find( path );
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
            ILoadResult preloadJob = null;

            //  ローカルにキャッシュがあったらそっちをロードする
            if ( CacheDatabase.HasCache( bundleData ))
            {
                preloadJob = DoLoadCacheWithNeedAll( bundleData );
            }
            else
            {
				//  なかったらサーバからダウンロードして
				//	ダウンロード済みファイルをローカルからロード
				preloadJob = DoDownloadWithNeedAll( bundleData );
            }
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
                return DoDownload( data );
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
            var preloadJob  = new ILoadResult<byte[]>[ dependencies.Length ];
            for( int i = 0; i < preloadJob.Length; i++ )
            {
                preloadJob[i] = DoDownload( dependencies[i] );
            }
            return preloadJob
                .ToParallel()
                .ToJoin( () => DoDownload( data ))
                .AsEmpty();
        }

		/// <summary>
		/// ダウンロード処理
		/// </summary>
		protected virtual ILoadResult<byte[]> DoDownload( TRuntimeData data )
        {
            var location    = LoadDatabase.ToBundleLocation( data );
            var job         = JobCreator.BytesLoad( JobEngine, location );
            return new LoadResult<byte[]>(
                job,
                onCompleted: ( content ) =>
                {
					Debug.Log( location.AccessPath );
					var ab = AssetBundle.LoadFromMemory( content );
					data.OnMemory( ab );
					CacheDatabase.SaveVersion( data, content );
					CacheDatabase.Apply();
				},
                dispose : LoadDatabase.AddReference( data )
            );
        }

		/// <summary>
		/// ローカルからロード
		/// </summary>
		protected virtual ILoadResult DoLoadCacheWithNeedAll( TRuntimeData bundleData )
		{
			if( bundleData.Dependencies.Length == 0 )
			{
				return DoLocalOpen( bundleData );
			}
			return DoLocalOpenDependencies( bundleData );
		}
		/// <summary>
		/// 事前ロードのみ
		/// </summary>
		protected virtual ILoadResult DoLocalOpenDependencies( TRuntimeData data )
		{
			//  事前に必要な分を合成
			var dependencies= data.Dependencies;
			var preloadJob  = new ILoadResult<AssetBundle>[ dependencies.Length ];
			for( int i = 0; i < preloadJob.Length; i++ )
			{
				preloadJob[i] = DoLocalOpen( dependencies[i] );
			}
			return preloadJob
				.ToParallel()
				.ToJoin( () => DoLocalOpen( data ) )
				.AsEmpty();
		}
		protected virtual ILoadResult<AssetBundle> DoLocalOpen( TRuntimeData data )
		{
			var location	= CacheDatabase.ToLocation( data.Name );
			var job			= JobCreator.OpenLocalBundle( JobEngine, location );
			return new LoadResult<AssetBundle>(
				job,
				onCompleted: ( content ) =>
				{
					Debug.Log( location.AccessPath );
					data.OnMemory( content );
				},
				dispose: LoadDatabase.AddReference( data )
			);
		}
	}
}
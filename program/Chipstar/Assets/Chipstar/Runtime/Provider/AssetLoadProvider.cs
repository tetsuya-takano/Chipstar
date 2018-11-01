﻿using System;
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
        IEnumerator     InitLoad    ( string fileName );
        ILoadResult<T>  LoadAsset<T>( string path     ) where T : UnityEngine.Object;
        IDisposable     LoadLevel   ( string sceneName);

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
        private IEntryPoint                     AccessPoint     { get; set; } // 接続先
        private ICacheDatabase                  CacheDatabase   { get; set; } // ローカルストレージのキャッシュ情報
        private ILoadDatabase<TRuntimeData>     LoadDatabase    { get; set; } // コンテンツテーブルから作成したDB
        private IJobEngine                      JobEngine       { get; set; } // DLエンジン
        private IJobCreator<TRuntimeData>       JobCreator      { get; set; } // ジョブの作成

        //===============================
        //  関数
        //===============================

        public AssetLoadProvider
            ( 
                IEntryPoint                accessPoint,
                ILoadDatabase<TRuntimeData> database,
                IJobEngine                  dlEngine,
                IJobCreator<TRuntimeData>   jobCreator
            )
        {
            AccessPoint     = accessPoint;
            LoadDatabase    = database;
            JobEngine       = dlEngine;
            JobCreator      = jobCreator;
        }

        public IEnumerator InitLoad( string fileName )
        {
            var job = InitielizeLoad( AccessPoint.ToLocation( fileName  ) );
            while( !job.IsCompleted )
            {
                yield return null;
            }

            LoadDatabase.Initialize( job.Content );

            yield break;
        }

        private ILoadJob<byte[]> InitielizeLoad( IAccessLocation location )
        {
            return JobCreator.CreateBytesLoad( JobEngine, location ); ;
        }

        /// <summary>
        /// パスからアセットの取得
        /// </summary>
        public virtual ILoadResult<T> LoadAsset<T>( string path ) where T : UnityEngine.Object
        {
            var data = LoadDatabase.Find( path );
            return DoLoadAsset<T>( data );
        }
        /// <summary>
        /// アセットの取得
        /// </summary>
        protected virtual ILoadResult<T> DoLoadAsset<T>( AssetData<TRuntimeData> assetData ) where T : UnityEngine.Object
        {
            //  パスからバンドルデータを取得
            var data    = assetData.BundleData;
            var path    = assetData.Path;
            if( data.IsOnMemory )
            {
                return CreateLoadAsset<T>( assetData );
            }
            return DoLoadAssetWithNeedAll<T>( assetData );
        }

        /// <summary>
        /// アセットを取得するため必要なデータを一通りロード
        /// </summary>
        protected virtual ILoadResult<T> DoLoadAssetWithNeedAll<T>( AssetData<TRuntimeData> data ) where T : UnityEngine.Object
        {
            var bundleData = data.BundleData;
            ILoadResult preloadJob = null;

            //  ローカルにキャッシュがあったらそっちをロードする
            if ( CacheDatabase.HasCache( bundleData ))
            {
                preloadJob = DoOpenLocalBundleDependencies( bundleData );
            }
            else
            {
                //  なかったらサーバからダウンロード
                preloadJob = DoDownloadWithNeedAll(bundleData);
            }
            //  ロード
            return preloadJob.ToJoin( () => CreateLoadAsset<T>( data ) );
        }

        /// <summary>
        /// ローカルからロード
        /// </summary>
        protected virtual ILoadResult DoOpenLocalBundleDependencies(TRuntimeData bundleData)
        {
            return null;
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
            var preloadJob  = new ILoadResult<AssetBundle>[ dependencies.Length ];
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
        protected virtual ILoadResult<AssetBundle> DoDownload( TRuntimeData data )
        {
            var location    = AccessPoint.ToLocation( data );
            var job         = JobCreator.CreateBundleFile( JobEngine, location );

            return new LoadResult<AssetBundle>(
                job,
                onCompleted: ( content ) =>
                {
                    data.OnMemory( content );
                },
                dispose : LoadDatabase.AddReference( data )
            );
        }

        /// <summary>
        /// 
        /// </summary>
        private ILoadResult<T> CreateLoadAsset<T>( AssetData<TRuntimeData> assetData )
            where T          : UnityEngine.Object
        {
            var job = JobCreator.CreateAssetLoad<T>( JobEngine, assetData );
            return new LoadResult<T>( 
                job         : job,
                onCompleted : j => { },
                dispose     : LoadDatabase.AddReference( assetData.BundleData )
            );
        }


        public IDisposable LoadLevel(string sceneName)
        {
            return null;
        }

        public void DoUpdate()
        {
            JobEngine.Update( );
        }
    }
}
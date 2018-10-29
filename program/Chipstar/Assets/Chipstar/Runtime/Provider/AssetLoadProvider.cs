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
        IEnumerator   InitLoad    ( string fileName );
        IDisposable   LoadAsset<T>( string path,        Action<T> onLoaded ) where T : UnityEngine.Object;
        IDisposable   LoadLevel   ( string sceneName );

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
        private IAccessPoint                    AccessPoint     { get; set; } // 接続先
        private ILoadDatabase<TRuntimeData>     LoadDatabase    { get; set; } // コンテンツテーブルから作成したDB
        private IJobEngine                      JobEngine       { get; set; } // DLエンジン
        private IJobCreator<TRuntimeData>       JobCreator      { get; set; } // ジョブの作成

        //===============================
        //  関数
        //===============================

        public AssetLoadProvider
            ( 
                IAccessPoint                accessPoint,
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
        public virtual IDisposable LoadAsset<T>( string path, Action<T> onLoaded ) where T : UnityEngine.Object
        {
            var data = LoadDatabase.Find( path );
            return DoLoadAsset( data, onLoaded );
        }
        /// <summary>
        /// アセットの取得
        /// </summary>
        protected virtual IDisposable DoLoadAsset<T>( AssetData<TRuntimeData> assetData, Action<T> onLoaded ) where T : UnityEngine.Object
        {
            //  パスからバンドルデータを取得
            var data    = assetData.BundleData;
            var path    = assetData.Path;
            if( data.IsOnMemory )
            {
                CreateLoadAsset( assetData, onLoaded );
                return LoadDatabase.AddReference( data );
            }
            return DoLoadAssetWithNeedAll( assetData, onLoaded );
        }

        /// <summary>
        /// アセットを取得するため必要なデータを一通りロード
        /// </summary>
        protected virtual IDisposable DoLoadAssetWithNeedAll<T>( AssetData<TRuntimeData> data, Action<T> onLoaded ) where T : UnityEngine.Object
        {
            return DoDownloadWithNeedAll( data.BundleData ).OnComplete( () => CreateLoadAsset( data, onLoaded ) );
        }

        /// <summary>
        /// 依存アセットバンドルを一通りロード
        /// </summary>
        protected virtual ILoadResult DoDownloadWithNeedAll( TRuntimeData data )
        {
            var preloadJob = DoDownloadDependencies( data );
            return preloadJob.ToJoin( () => DoDownload( data ) );
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
                preloadJob[i] = DoDownload( dependencies[i] );
            }
            return preloadJob.ToParallel();
        }

        /// <summary>
        /// ダウンロード処理
        /// </summary>
        protected virtual ILoadResult DoDownload( TRuntimeData data )
        {
            var location    = AccessPoint.ToLocation( data );
            var job         = JobCreator.CreateBundleFile( JobEngine, location );

            return new LoadResult<AssetBundle>(
                job,
                onCompleted: ( j ) =>
                {
                    data.OnMemory( j.Content );
                },
                dispose : LoadDatabase.AddReference( data )
            );
        }

        /// <summary>
        /// 
        /// </summary>
        private void CreateLoadAsset<T>( AssetData<TRuntimeData> assetData, Action<T> onLoaded )
            where T          : UnityEngine.Object
        {
            var job = JobCreator.CreateAssetLoad( JobEngine, assetData );
            job.OnLoaded = () =>
            {
                onLoaded( job.Content as T );
            };
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
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
        IEnumerator   InitLoad    ( UrlLocation location );
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
        private ILoadDatabase<TRuntimeData>     LoadDatabase    { get; set; } // コンテンツテーブルから作成したDB
        private IJobEngine                      JobEngine        { get; set; } // DLエンジン
        private IJobCreator                     JobCreator      { get; set; } // ジョブの作成

        //===============================
        //  関数
        //===============================

        public AssetLoadProvider
            ( 
                ILoadDatabase<TRuntimeData> database,
                IJobEngine                 dlEngine,
                IJobCreator                 jobCreator
            )
        {
            LoadDatabase    = database;
            JobEngine        = dlEngine;
            JobCreator      = jobCreator;
        }

        public IEnumerator InitLoad( UrlLocation location )
        {
            var job = InitielizeLoad( location );
            while( job.IsCompleted )
            {
                yield return null;
            }

            LoadDatabase.Initialize( job.Content );

            yield break;
        }

        private ILoadJob<string> InitielizeLoad( UrlLocation location )
        {
            var job = JobCreator.CreateTextLoad( location );

            JobEngine.Enqueue(job);

            return job;
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
                CreateLoadAsset<T>( assetData, onLoaded);
                return LoadDatabase.AddReference( data );
            }
            return DoDownload( data, path, () => CreateLoadAsset<T>( assetData, onLoaded ) );
        }

        protected virtual IDisposable DoDownload( TRuntimeData data, string path, Action onLoaded )
        {
            var job = JobCreator.CreateBundleFile( new UrlLocation( "" ));
            job.OnLoaded = () =>
            {
                data.OnMemory( job.Content );
                onLoaded();
            };
            JobEngine.Enqueue(job);

            return LoadDatabase.AddReference( data );
        }
        
        /// <summary>
        /// 
        /// </summary>
        private void CreateLoadAsset<T>( AssetData<TRuntimeData> assetData, Action<T> onLoaded ) where T : UnityEngine.Object
        {
            var job = JobCreator.CreateAssetLoad( assetData.Path );
            job.OnLoaded = () => onLoaded( job.Content as T );
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
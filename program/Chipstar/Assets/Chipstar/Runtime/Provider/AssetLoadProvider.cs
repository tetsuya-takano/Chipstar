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
        private ILoadEngine                     DLEngine        { get; set; } // DLエンジン
        private ILoadEngine                     LoadEngine      { get; set; } // DLエンジン
        private IJobCreator                     JobCreator      { get; set; } // ジョブの作成

        //===============================
        //  関数
        //===============================

        public AssetLoadProvider
            ( 
                ILoadDatabase<TRuntimeData> database,
                ILoadEngine                 dlEngine,
                ILoadEngine                 loadEngine,
                IJobCreator                 jobCreator
            )
        {
            LoadDatabase    = database;
            DLEngine        = dlEngine;
            LoadEngine      = loadEngine;
            JobCreator      = jobCreator;
        }

        public IEnumerator InitLoad( UrlLocation location )
        {
            var job = InitielizeLoad( location );

            var waitForInit = new WaitUntil(() => job.IsCompleted );
            yield return waitForInit;

            LoadDatabase.Initialize( job.Content );

            yield break;
        }

        private ILoadJob<string> InitielizeLoad(  UrlLocation location )
        {
            var job = JobCreator.CreateTextLoad( location );

            DLEngine.Enqueue(job);

            return job;
        }

        /// <summary>
        /// アセットの取得
        /// </summary>
        public IDisposable LoadAsset<T>( string path, Action<T> onLoaded ) where T : UnityEngine.Object
        {
            //  パスからバンドルデータを取得
            var data    = LoadDatabase.Find( path );
            if( data.IsOnMemory )
            {
                CreateLoadAsset<T>(data, path, onLoaded);
                return null;
            }
            return DoDownload( data, path, () => CreateLoadAsset<T>(data, path, onLoaded ) );
        }

        protected virtual IDisposable DoDownload( TRuntimeData data, string path, Action onLoaded )
        {
            var job = JobCreator.CreateBundleFile(new UrlLocation(path));
            job.OnLoaded = () =>
            {
                data.OnMemory( job.Content );
                onLoaded();
            };
            DLEngine.Enqueue(job);

            return null;
        }
        
        /// <summary>
        /// 
        /// </summary>
        private void CreateLoadAsset<T>( TRuntimeData data, string path, Action<T> onLoaded ) where T : UnityEngine.Object
        {
            var job = JobCreator.CreateAssetLoad(path);
            job.OnLoaded = () => onLoaded( job.Content as T );

            LoadEngine.Enqueue( job );
        }


        public IDisposable LoadLevel(string sceneName)
        {
            return null;
        }

        public void DoUpdate()
        {
            DLEngine    .Update();
            LoadEngine  .Update();
        }
    }
}
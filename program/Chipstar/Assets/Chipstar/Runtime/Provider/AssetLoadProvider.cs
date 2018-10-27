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
        ILoadJob<T>   LoadAsset<T>( string path );
        ILoadJob      LoadLevel   ( string sceneName );
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
        private IDownloadEngine                 Engine          { get; set; } // DLエンジン
        private IJobCreator                     JobCreator      { get; set; } // ジョブの作成

        //===============================
        //  関数
        //===============================

        public IEnumerator InitLoad()
        {
            var job = JobCreator.CreateTextLoad( new UrlLocation( "" ) );

            Engine.Enqueue( job );

            var waitForInit = new WaitWhile(() => job.IsCompleted );
            yield return waitForInit;

            LoadDatabase.Initialize( job.Content );
        }

        /// <summary>
        /// アセットの取得
        /// </summary>
        public ILoadJob<T> LoadAsset<T>(string path)
        {
            //  パスからバンドルデータを取得
            var data    = LoadDatabase.Find( path );
            if( data.IsOnMemory )
            {
                return null;
            }

            var job = JobCreator.CreateBundleFile( new UrlLocation( "" ) );
            Engine.Enqueue( job );

            return null;
        }
        public ILoadJob LoadLevel(string sceneName)
        {

            return null;
        }
    }
}
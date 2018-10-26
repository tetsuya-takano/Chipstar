namespace Chipstar.Downloads
{
    //==============================
    //  読み込み統括
    //  リクエストを受け取って、タスクを返す
    //  使用者が触るのはこれ
    //==============================


    public interface IAssetLoadProvider
    {
        IAssetLoadTask<T>   LoadAsset<T>( string path );
        ISceneLoadTask      LoadLevel   ( string sceneName );
    }

    /// <summary>
    /// 読み込みマネージャ
    /// </summary>
    public class AssetLoadProvider<TData> 
                    : IAssetLoadProvider
        where TData : IRuntimeBundleData
    {
        private ILoadDatabase<TData>    LoadDatabase    { get; set; }
        private ILoadManager            LoadManager     { get; set; }


        /// <summary>
        /// アセットの取得
        /// </summary>
        public IAssetLoadTask<T> LoadAsset<T>(string path)
        {
            //  パスからバンドルデータを取得
            var data    = LoadDatabase.Find( path );
            if( data.IsOnMemory )
            {
                return LoadManager.LoadAsset<T>( data );
            }
            return null;
        }

        public ILoadTask Preload( string path )
        {
            var data    = LoadDatabase.Find( path );
            return DoPreload( data );
        }

        protected virtual ILoadTask DoPreload( IRuntimeBundleData data )
        {
            return LoadManager.PreloadBundleFile( data );
        }

        public ISceneLoadTask LoadLevel(string sceneName)
        {
            return null;
        }
    }
}
namespace Chipstar.Downloads
{
    public interface IAssetLoadProvider
    {
        IAssetLoadTask<T>   LoadAsset<T>( string path );
        ISceneLoadTask      LoadLevel   ( string sceneName );
    }

    /// <summary>
    /// 読み込みマネージャ
    /// </summary>
    public class AssetLoadProvider : IAssetLoadProvider
    {
        private ILoadDatabase   LoadDatabase    { get; set; }
        private ILoadManager    LoadManager     { get; set; }


        /// <summary>
        /// アセットの取得
        /// </summary>
        public IAssetLoadTask<T> LoadAsset<T>(string path)
        {
            //  パスからバンドルデータを取得
            var data    = LoadDatabase.Find( path );
            return LoadManager.Create<T>( data );
        }

        public ISceneLoadTask LoadLevel(string sceneName)
        {
            return null;
        }
    }
}
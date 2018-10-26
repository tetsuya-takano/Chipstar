namespace Chipstar.Downloads
{
    internal interface ILoadManager
    {
        IAssetLoadTask<T> Create<T>( IRuntimeBundleData data );
    }


    public class LoadManager : ILoadManager
    {
        private IDownloadEngine Engine { get; set; }
        /// <summary>
        /// 読み込み管理
        /// </summary>
        public IAssetLoadTask<T> Create<T>( IRuntimeBundleData data)
        {
            //  依存先
            var dependencies = data.Dependencies;
            //  必要なもの
            var main         = data;

            foreach (var bundle in dependencies)
            {
                if (bundle.IsOnMemory)
                {
                    continue;
                }
                Engine.Enqueue( bundle );
            }
        }

        public void Update()
        {
            Engine.Update();
        }
    }
}
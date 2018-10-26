namespace Chipstar.Downloads
{
    internal interface ILoadManager
    {
        IAssetLoadTask<T>   LoadAsset<T>     ( IRuntimeBundleData data );
        ILoadTask           PreloadBundleFile( IRuntimeBundleData data );
    }


    public class LoadManager : ILoadManager
    {
        //==================================
        //
        //==================================
        private IDownloadEngine     DLEngine        { get; set; }
        private IRequestConverter   ReqConverter    { get; set; }

        //==================================
        //
        //==================================

        /// <summary>
        /// 読み込み管理
        /// </summary>
        public IAssetLoadTask<T> LoadAsset<T>( IRuntimeBundleData data)
        {
            return CreateLoadAsset<T>( data );
        }

        public ILoadTask PreloadBundleFile( IRuntimeBundleData data )
        {
            return CreateFileLoadWithNeedAll( data );
        }

        protected virtual IAssetLoadTask<T> CreateLoadAsset<T>( IRuntimeBundleData data )
        {
            return 
        }


        protected virtual ILoadTask CreateFileLoad( IRuntimeBundleData data )
        {
            var req  = ReqConverter .Create( data );
            var task = DLEngine     .Enqueue( req );

            return task;
        }

        protected virtual ILoadTask CreateFileLoadWithNeedAll( IRuntimeBundleData data )
        {
            var dependencies = data.Dependencies;
            foreach( var b in dependencies )
            {
                var task = CreateFileLoad( b );
            }
            var mainTask = CreateFileLoad( data );

            return mainTask;
        }

        public void Update()
        {
            DLEngine.Update();
        }
    }
}
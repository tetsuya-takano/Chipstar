using System;

namespace Chipstar.Downloads
{
    public interface ILoadManager : IDisposable
    {
        IAssetLoadTask<T>   LoadAsset<T>     ( IRuntimeBundleData data );
        ILoadTask           PreloadBundleFile( IRuntimeBundleData data );

        void Update();
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
        public void Dispose()
        {
            DLEngine.Dispose();
        }

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
            return null; 
        }


        protected virtual ILoadTask CreateFileLoad( IRuntimeBundleData data )
        {
            var job = ReqConverter.Create( data );
            DLEngine.Enqueue( job );

            return null;
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
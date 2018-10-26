using UnityEngine;
using System.Collections;
namespace Chipstar.Downloads
{
    public interface IRuntimeBundleData
    {
        string                  Name        { get; }
        string[]                Assets      { get; }
        IRuntimeBundleData[]    Dependencies{ get; }
        bool                    IsOnMemory  { get; }

        AsyncOperation LoadAsync<T>( string path );
        void Unload();
    }
    public class BundleData : IRuntimeBundleData
    {
        public      string               Name        { get; private set; }
        public      string[]             Assets      { get; private set; }
        public      IRuntimeBundleData[] Dependencies{ get; private set; }
        public      bool                 IsOnMemory  { get { return Bundle != null; } }

        protected   AssetBundle          Bundle      { get; set; }

        public AsyncOperation LoadAsync<T>( string path )
        {
            return Bundle.LoadAssetAsync<T>( path );
        }

        public void Unload()
        {
            if (Bundle != null)
            {
                Bundle.Unload(true);
            }
            Bundle = null;
        }
    }
}
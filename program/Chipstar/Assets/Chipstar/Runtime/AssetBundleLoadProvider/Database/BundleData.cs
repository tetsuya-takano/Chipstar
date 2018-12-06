using UnityEngine;
using System.Collections;
using System;

namespace Chipstar.Downloads
{
    public interface IRuntimeBundleData<T> 
		: IDisposable,
		ICachableBundle,
		IRefCountable
        where T : IRuntimeBundleData<T>
    {
        AssetData<T>[]  Assets      { get; }
        T[]             Dependencies{ get; }
        bool            IsOnMemory  { get; }
		bool            IsScene		{ get; }
		uint			Crc			{ get; }

		AssetBundleRequest LoadAsync<TAssetType>( string path ) where TAssetType : UnityEngine.Object;
		void Unload();
        void OnMemory( AssetBundle bundle );

        void Set( IBundleBuildData	data		 );
        void Set( AssetData<T>[]	assets		 );
        void Set( T[]				dependencies );
    }

    public class AssetData<T> where T : IRuntimeBundleData<T>
    {
        public string Path       { get; private set; }
        public string Guid       { get; private set; }
        public T      BundleData { get; private set; }

        public AssetData( IAssetBuildData data )
        {
			Apply( data.Path, data.Guid );
		}
        public void Apply( string path, string guid )
        {
            Path = path;
            Guid = guid;
        }
        public void Connect( T data )
        {
            BundleData = data;
        }

		internal AssetBundleRequest LoadAsync<TAssetType>() where TAssetType : UnityEngine.Object
		{
			return BundleData.LoadAsync<TAssetType>( Path );
		}
	}

    public abstract class BundleData<T> 
        :   IRuntimeBundleData<T> 
            where T : IRuntimeBundleData<T>
    {
        //========================================
        //  プロパティ
        //========================================

        public      string          Name        { get; private set; }
        public      AssetData<T>[]  Assets      { get; private set; }
        public      Hash128         Hash        { get; private set; }
		public      uint			Crc			{ get; private set; }
        public      T[]             Dependencies{ get; private set; }
        public      bool            IsOnMemory  { get { return Bundle != null; } }
		public      bool            IsScene		{ get { return Bundle.isStreamedSceneAssetBundle; } }

        public      bool            IsFree      { get { return RefCount <= 0; } }

        protected   AssetBundle     Bundle      { get; set; }
        public		int             RefCount    { get; private set; }

        //========================================
        //  関数
        //========================================

        public void Dispose()
        {
            Unload();
            Assets       = null;
            Dependencies = null;

        }

        public void Set( IBundleBuildData data )
        {
            Name = data.ABName;
            Hash = Hash128.Parse( data.Hash );
			Crc  = data.Crc;
        }

        public void Set(AssetData<T>[] assets)
        {
            Assets = assets;
        }

        public void Set(T[] dependencies)
        {
            Dependencies = dependencies;
        }

        /// <summary>
        /// アセットバンドル保持
        /// </summary>
        public void OnMemory( AssetBundle bundle )
        {
            Bundle = bundle;
        }

		/// <summary>
		/// 読み込み
		/// </summary>
		public AssetBundleRequest LoadAsync<TAssetType>( string path ) where TAssetType : UnityEngine.Object
		{
            return Bundle.LoadAssetAsync<TAssetType>( path );
        }

        public void Unload()
        {
            if (Bundle != null)
            {
                Bundle.Unload(true);
            }
            Bundle = null;
        }

        /// <summary>
        /// 参照カウンタ加算
        /// </summary>
        public void AddRef()
        {
            RefCount++;
			if( Dependencies == null ) { return; }
			for( int i = 0; i < Dependencies.Length; i++ )
			{
				Dependencies[ i ].AddRef();
			}
        }

        /// <summary>
        /// 参照カウンタ減算
        /// </summary>
        public void ReleaseRef()
        {
            RefCount = Mathf.Max( 0, RefCount - 1 );
			if( Dependencies == null ) { return; }
			for( int i = 0; i < Dependencies.Length; i++ )
			{
				Dependencies[i].ReleaseRef();
			}
		}

		/// <summary>
		/// 参照カウンタ破棄
		/// </summary>
		public void ClearRef()
        {
            RefCount = 0;
        }

		public override string ToString()
		{
			return this.ToCacheDataStr();
		}
	}
}
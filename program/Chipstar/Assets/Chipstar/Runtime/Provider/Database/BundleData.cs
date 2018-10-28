using UnityEngine;
using System.Collections;
using System;

namespace Chipstar.Downloads
{
    public interface IRuntimeBundleData<T> : IDisposable
        where T : IRuntimeBundleData<T>
    {
        string          Name        { get; }
        AssetData<T>[]  Assets      { get; }
        Hash128         Hash        { get; }
        T[]             Dependencies{ get; }
        bool            IsOnMemory  { get; }

        AssetBundleRequest LoadAsync<TAsset>( string path );
        void Unload();
        void OnMemory( AssetBundle bundle );

        void Set( string abName, string hash );
        void Set( AssetData<T>[] assets );
        void Set( T[]         dependencies );
    }

    public class AssetData<T> where T : IRuntimeBundleData<T>
    {
        public string Path       { get; private set; }
        public string Guid       { get; private set; }
        public T      BundleData { get; private set; }

        public AssetData(string path, string guid)
        {
            Apply(path, guid );
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
        public      T[]             Dependencies{ get; private set; }
        public      bool            IsOnMemory  { get { return Bundle != null; } }

        protected   AssetBundle     Bundle      { get; set; }

        //========================================
        //  関数
        //========================================

        public void Dispose()
        {
            Unload();
            Assets       = null;
            Dependencies = null;

        }

        public void Set( string abName, string hash )
        {
            Name = abName;
            Hash = Hash128.Parse( hash );
        }

        public void Set(AssetData<T>[] assets)
        {
            Assets = assets;
        }

        public void Set(T[] dependencies)
        {
            Dependencies = dependencies;
        }



        public void OnMemory( AssetBundle bundle )
        {
            Bundle = bundle;
        }
        public AssetBundleRequest LoadAsync<TAsset>( string path )
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
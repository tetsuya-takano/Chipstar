﻿using UnityEngine;
using System;

namespace Chipstar.Downloads
{
    public interface IJobCreator : IDisposable
    {
        ILoadJob<string>            CreateTextLoad    ( UrlLocation location );
        ILoadJob<AssetBundle>       CreateBundleFile  ( UrlLocation location );
        ILoadJob<UnityEngine.Object>CreateAssetLoad   ( string      location );
    }
    public class JobCreator : IJobCreator
    {
        //=======================================
        //  変数
        //=======================================
        protected Func<IAccessLocation, ILoadJob<string>>             OnTextLoad     { get; set; }
        protected Func<IAccessLocation, ILoadJob<AssetBundle>>        OnBundleLoad   { get; set; }
        protected Func<IAccessLocation, ILoadJob<UnityEngine.Object>> OnAssetLoad    { get; set; }

        //=======================================
        //  関数
        //=======================================
        public JobCreator( 
            Func<IAccessLocation, ILoadJob<string>>             onTextLoad,
            Func<IAccessLocation, ILoadJob<AssetBundle>>        onBundleLoad,
            Func<IAccessLocation, ILoadJob<UnityEngine.Object>> onAssetLoad
        )
        {
            OnTextLoad   = onTextLoad;
            OnBundleLoad = onBundleLoad;
            OnAssetLoad  = onAssetLoad;
        }

        public void Dispose()
        {
            OnTextLoad = null;
        }

        /// <summary>
        /// テキスト取得リクエスト
        /// </summary>
        public ILoadJob<string> CreateTextLoad(UrlLocation location)
        {
            return OnTextLoad( location );
        }

        public ILoadJob<AssetBundle> CreateBundleFile(UrlLocation location)
        {
            return OnBundleLoad( location );
        }

        public ILoadJob<UnityEngine.Object> CreateAssetLoad( string location )
        {
            return OnAssetLoad( new UrlLocation( location ) );
        }
    }
}
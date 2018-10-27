using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace Chipstar.Downloads
{
    public interface IJobCreator : IDisposable
    {
        ILoadJob<string>      CreateTextLoad  ( UrlLocation location );
        ILoadJob<AssetBundle> CreateBundleFile( UrlLocation location );
    }
    public class JobCreator : IJobCreator
    {
        //=======================================
        //  変数
        //=======================================
        protected Func<UrlLocation, ILoadJob<string>>      OnTextLoad     { get; set; }
        protected Func<UrlLocation, ILoadJob<AssetBundle>> OnBundleLoad   { get; set; }

        //=======================================
        //  関数
        //=======================================
        public JobCreator( 
            Func<UrlLocation, ILoadJob<string>>       onTextLoad,
            Func<UrlLocation, ILoadJob<AssetBundle>>  onBundleLoad
        )
        {
            OnTextLoad   = onTextLoad;
            OnBundleLoad = onBundleLoad;
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
    }
}
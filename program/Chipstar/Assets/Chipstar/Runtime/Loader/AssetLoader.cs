using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chipstar.Downloads
{
    public static class AssetLoader
    {
        private static IAssetLoadProvider LoadProvider { get; set; }


        /// <summary>
        /// 読み込み
        /// </summary>
        public static ILoadTask<T> Load<T>( string path )
        {
            return LoadProvider.LoadAsset<T>( path );
        }
    }
}
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
        public static IDisposable Load<T>( string path, Action<T> onLoaded  ) where T : UnityEngine.Object
        {
			return null;
        }
    }
}
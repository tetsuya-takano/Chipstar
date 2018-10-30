using UnityEngine;
using System.Collections;
namespace Chipstar.Downloads
{
    public static class AssetLoad
    {
        /// <summary>
        /// アセットのロードで取る機能
        /// </summary>
        public abstract class AssetLoadHandler<T>
            : DLHandler<AssetBundleRequest, T>
            where T : UnityEngine.Object
        {
        }

        public sealed class AsyncLoad<T> : AssetLoadHandler<T> where T : UnityEngine.Object
        {
            protected override T DoComplete(AssetBundleRequest source)
            {
                return source.asset as T;
            }
        }

    }
}
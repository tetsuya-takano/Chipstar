using UnityEngine;
using System.Collections;
namespace Chipstar.Downloads
{
    public static class AssetLoad
    {
        /// <summary>
        /// アセットのロードで取る機能
        /// </summary>
        public abstract class AssetLoadHandler
            : DLHandler<AssetBundleRequest, UnityEngine.Object>
        {
        }

        public sealed class AsyncLoad : AssetLoadHandler
        {
            protected override UnityEngine.Object DoComplete(AssetBundleRequest source)
            {
                return source.asset;
            }
        }

    }
}
using UnityEngine;
using System.Collections;
namespace Chipstar.Downloads
{
    public static class SceneLoader
    {
        private static IAssetLoadProvider LoadProvider { get; set; }


        public static ILoadTask LoadLevel( string sceneName )
        {
            return LoadProvider.LoadLevel( sceneName );
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace Chipstar.Builder
{
    /// <summary>
    /// ビルドしない
    /// </summary>
    public sealed class DisableBuildProcess<T> : ABBuildProcess<T> where T : IABBuildData
    {
        protected override AssetBundleManifest DoBuild( string outputPath, AssetBundleBuild[] bundleList, BuildAssetBundleOptions option, BuildTarget platform )
        {
            Debug.Log( " ----------------------------------- " );
            foreach( var b in bundleList)
            {
                Debug.Log( b.assetBundleName );
            }
            Debug.Log( " ----------------------------------- ");
            return null;
        }
    }
}
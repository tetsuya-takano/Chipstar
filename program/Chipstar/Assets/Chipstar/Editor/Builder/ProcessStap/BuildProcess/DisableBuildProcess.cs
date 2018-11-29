using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace Chipstar.Builder
{
    /// <summary>
    /// ビルドしない
    /// </summary>
    public sealed class DisableBuildProcess<T> : ABBuildProcess<T,SimpleResult> where T : IABBuildData
    {
        protected override SimpleResult DoBuild( string outputPath, AssetBundleBuild[] bundleList, BuildAssetBundleOptions option, BuildTarget platform )
        {
            Debug.Log( " ----------------------------------- " );
            foreach( var b in bundleList)
            {
                Debug.Log( b.assetBundleName );
            }
            Debug.Log( " ----------------------------------- ");
            return new SimpleResult( true );
        }
    }
}
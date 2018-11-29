using UnityEngine;
using UnityEditor;

namespace Chipstar.Builder
{

    public class SimpleABBuildProcess<TData> 
        : ABBuildProcess<TData, ABBuildResult>
        where TData : IABBuildData
    {
        public static SimpleABBuildProcess<TData> Empty = new SimpleABBuildProcess<TData>();

        protected override ABBuildResult DoBuild(string outputPath, AssetBundleBuild[] bundleList, BuildAssetBundleOptions option, BuildTarget platform)
        {
            var manifest = BuildPipeline.BuildAssetBundles( 
                outputPath          : outputPath, 
                assetBundleOptions  : option,
                targetPlatform      : platform,

                builds              : bundleList );

            return new ABBuildResult( manifest );
        }
    }
}
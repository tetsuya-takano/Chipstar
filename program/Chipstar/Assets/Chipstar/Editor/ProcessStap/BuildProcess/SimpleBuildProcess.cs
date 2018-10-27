using UnityEngine;
using UnityEditor;

namespace Chipstar.Builder
{

    public class SimpleBuildProcess<TData> 
        : ABBuildProcess<TData, SimpleResult>
        where TData : IABBuildData
    {
        public static SimpleBuildProcess<TData> Empty = new SimpleBuildProcess<TData>();

        protected override SimpleResult DoBuild(string outputPath, AssetBundleBuild[] bundleList, BuildAssetBundleOptions option, BuildTarget platform)
        {
            var manifest = BuildPipeline.BuildAssetBundles( 
                outputPath          : outputPath, 
                assetBundleOptions  : option,
                targetPlatform      : platform,

                builds              : bundleList );

            return new SimpleResult( manifest != null );
        }
    }
}
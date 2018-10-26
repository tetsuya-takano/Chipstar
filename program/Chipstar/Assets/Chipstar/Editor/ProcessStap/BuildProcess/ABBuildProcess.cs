using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Chipstar.Builder
{
    /// <summary>
    /// アセットバンドルビルド用インターフェース
    /// </summary>
    public interface IABBuildProcess<T> where T : IABBuildData
	{
		AssetBundleManifest Build( IABBuildConfig settings, IList<T> assetBundleList );
	}

    public class ABBuildProcess<T> : IABBuildProcess<T> where T : IABBuildData
    {
        public static readonly ABBuildProcess<T> Empty = new ABBuildProcess<T>();

        /// <summary>
        /// ビルド
        /// </summary>
        public virtual AssetBundleManifest Build( 
            IABBuildConfig      settings, 
            IList<T>            assetBundleList 
        )
        {
            var outputPath = settings.OutputPath;

            if( !Directory.Exists( outputPath ) )
            {
                Directory.CreateDirectory( outputPath );
            }

            var option     = settings.Options;
            var platform   = settings.BuildTarget;
            var bundleList = assetBundleList
                                .Select( d => new AssetBundleBuild
                                {
                                    assetBundleName = d.ABName,
                                    assetNames      = d.Assets
                                })
                                .ToArray();

            return DoBuild( 
                outputPath  : outputPath,
                option      : option,
                platform    : platform,

                bundleList  : bundleList
            );
        }

        protected virtual AssetBundleManifest DoBuild( 
            string                  outputPath,
            AssetBundleBuild[]      bundleList ,
            BuildAssetBundleOptions option,
            BuildTarget             platform
        )
        {
            return BuildPipeline.BuildAssetBundles( 
                outputPath          : outputPath,
                builds              : bundleList,
                assetBundleOptions  : option,
                targetPlatform      : platform
            );
        }
    }
}
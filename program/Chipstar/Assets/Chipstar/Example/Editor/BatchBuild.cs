using Chipstar.Builder;
using Chipstar.Example;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
namespace Chipstar.Example
{
    public static class BatchBuild
    {
        static void Build( string prefix, BuildAssetBundleOptions addOption )
        {
			var builder = AssetBundleBuilder.Default(
                packageConfigFile	:	"../settings/abPack.csv",
				
				buildTargetFolder	:	"Assets/BundleTarget/",
				outputPath			:	Path.Combine( Application.dataPath, "../../build/windows/" + prefix + "/" ),
				
				buildMapFile		:   "buildMap.json",
				options    : BuildAssetBundleOptions.ForceRebuildAssetBundle | BuildAssetBundleOptions.IgnoreTypeTreeChanges | addOption,
                platform   : BuildTarget.StandaloneWindows64
            );
			var result = builder.Build();
			if( result.IsSuccess )
			{
				Debug.Log( "[AssetBundle] Success!!" );
				return;
			}
			Debug.LogError( "[AssetBundle] Failure..." );
		}

        [MenuItem( "Tools/Build/LZ4" )]
        static void BuildLZ4()
        {
            Build( "lz4", BuildAssetBundleOptions.ChunkBasedCompression );
        }
        [MenuItem( "Tools/Build/LZMA" )]
        static void BuildLZMA()
        {
            Build( "lzma", BuildAssetBundleOptions.None );
        }
        [MenuItem( "Tools/Build/Raw" )]
        static void BuildRaw()
        {
            Build( "raw", BuildAssetBundleOptions.UncompressedAssetBundle );
        }
    }
}
﻿using Chipstar.Builder;
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
            var builder = new AssetBundleBuilder<ABPackageMst,ABBuildData, ABBuildResult>();
            var config  = new ABBuildConfig(
                outputPath : Path.Combine( Application.dataPath, "../../build/windows/" + prefix + "/"),
                option     : BuildAssetBundleOptions.ForceRebuildAssetBundle | BuildAssetBundleOptions.IgnoreTypeTreeChanges | addOption,
                platform   : BuildTarget.StandaloneWindows64
            );

            var fileFilter      = new ABBuildFileFilter( 
                ignorePattern: new string[] 
                {
                    ".cs", ".meta", ".asmdef"
                } 
            );
            //var buildProcess  = new DisableBuildProcess<ABBuildData>();
            var buildProcess    = SimpleABBuildProcess<ABBuildData>.Empty;

            builder.Build
                (
                    config          : config,
                    fileFilter      : fileFilter,
                    packageSettings : new ABPackageMstTable( "../settings/abPack.csv" ),
                    buildProcess    : buildProcess,
                    preProcess      : ABBuildPreProcess <ABBuildData>.Empty,
                    postProcess     : new SaveBuildMapPostProcess( "buildMap.json" )
                );
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
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
        [MenuItem( "Tools/Build" )]
        static void Hoge()
        {
            var builder = new AssetBundleBuilder<ABPackageMst,ABBuildData>();
            var config  = new ABBuildConfig(
                outputPath : Path.Combine( Application.dataPath, "../../build/windows/"),
                option     : BuildAssetBundleOptions.ForceRebuildAssetBundle,
                platform   : BuildTarget.StandaloneWindows64
            );

            var fileFilter      = new ABBuildFileFilter( 
                ignoreExtensions: new string[] 
                {
                    ".cs", ".meta"
                } 
            );
            //var buildProcess  = new DisableBuildProcess<ABBuildData>();
            var buildProcess    = ABBuildProcess<ABBuildData>.Empty;

            builder.Build
                (
                    config          : config,
                    fileFilter      : fileFilter,
                    packageSettings : new ABPackageMstTable( "../settings/abPack.csv" ),
                    buildProcess    : buildProcess,
                    postProcess     : ABBuildPostProcess<ABBuildData>.Empty,
                    preProcess      : ABBuildPreProcess <ABBuildData>.Empty
                );
        }
    }
}
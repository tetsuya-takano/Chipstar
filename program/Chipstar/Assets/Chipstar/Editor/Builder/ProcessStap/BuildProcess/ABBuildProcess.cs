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
    public interface IABBuildProcess<TData, TResult> 
        where TData     : IABBuildData
        where TResult   : IABBuildResult
    {
		TResult Build( IABBuildConfig settings, IList<TData> assetBundleList );
	}

    public interface IABBuildResult
    {
        bool IsSuccess { get; }

    }

    public abstract class ABBuildProcess<TData,TResult> : IABBuildProcess<TData, TResult> 
        where TData     : IABBuildData
        where TResult   : IABBuildResult
    {
        /// <summary>
        /// ビルド
        /// </summary>
        public virtual TResult Build( 
            IABBuildConfig      settings, 
            IList<TData>        assetBundleList 
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
                                .Select( d => d.ToBuildEntry() )
                                .ToArray();

            return DoBuild( 
                outputPath  : outputPath,
                option      : option,
                platform    : platform,

                bundleList  : bundleList
            );
        }

        protected abstract TResult DoBuild( 
            string                  outputPath,
            AssetBundleBuild[]      bundleList ,
            BuildAssetBundleOptions option,
            BuildTarget             platform
        );
    }
}
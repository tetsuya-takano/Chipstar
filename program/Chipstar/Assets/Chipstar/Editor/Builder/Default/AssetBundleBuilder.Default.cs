using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Chipstar.Builder
{
	/// <summary>
	/// アセットバンドルビルドデフォルト実装
	/// </summary>
	public static partial class AssetBundleBuilder
	{
		/// <summary>
		/// デフォルトビルドを返却
		/// </summary>
		public static IAssetBundleBuilder<TResult> Default<TResult>()
		{
			return null;
		}

		private sealed class AssetBundleDefaultBuilder
		{
			public AssetBundleDefaultBuilder()
			{
				var config  = new ABBuildConfig(
					outputPath : Path.Combine( Application.dataPath, "../../build/windows/" + prefix + "/"),
					option     : BuildAssetBundleOptions.ForceRebuildAssetBundle | BuildAssetBundleOptions.IgnoreTypeTreeChanges | addOption,
					platform   : BuildTarget.StandaloneWindows64
				);

				var fileFilter      = new ABBuildFileFilter(
					ignorePattern: new string[]
					{
						"(.*).cs", "(.*).meta", "(.*).asmdef",	//	無視ファイル
						"(.*)Resources/"						//	無視フォルダ
					}
				);
				//var buildProcess  = new DisableBuildProcess<ABBuildData>();
				var buildProcess    = SimpleABBuildProcess<ABBuildData>.Empty;
				var builder = new AssetBundleBuilder<ABPackageMst,ABBuildData, ABBuildResult>
					(
						config          : config,
						fileFilter      : fileFilter,
						packageSettings : new ABPackageMstTable( "../settings/abPack.csv" ),
						buildProcess    : buildProcess,
						preProcess      : ABBuildPreProcess <ABBuildData>.Empty,
						postProcess     : new SaveBuildMapPostProcess( "buildMap.json" )
					);
			}
		}
	}
}
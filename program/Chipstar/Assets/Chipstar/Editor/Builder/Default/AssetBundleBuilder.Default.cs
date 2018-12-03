using System.Collections;
using System.Collections.Generic;
using UnityEditor;
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
		public static IAssetBundleBuilder<ABBuildResult> Default
			( 
				string					buildTargetFolder,
				string					packageConfigFile,
				string					outputPath,
				string					buildMapFile,
				BuildAssetBundleOptions	options,
				BuildTarget				platform
			)
		{
			return new AssetBundleDefaultBuilder( packageConfigFile, buildMapFile, buildTargetFolder, outputPath, options, platform );
		}

		/// <summary>
		/// カスタマイズナシのビルドクラス
		/// </summary>
		private sealed class AssetBundleDefaultBuilder : AssetBundleBuilder<ABPackageMst, ABBuildData, ABBuildResult>
		{
			//===================================
			//	関数
			//===================================

			/// <summary>
			/// コンストラクタ
			/// </summary>
			public AssetBundleDefaultBuilder
				( 
					string					packageConfigFile,
					string					buildMapFile,

					string					buildTargetPath,
					string					outputPath,
					BuildAssetBundleOptions	options,
					BuildTarget				platform
				) : base
				(
					config		: new ABBuildConfig( buildTargetPath, outputPath, platform, options ),
					fileFilter	: new ABBuildFileFilter(
					ignorePattern: new string[]
					{
						"(.*).cs", "(.*).meta", "(.*).asmdef",	//	無視ファイル
						"(.*)Resources/"						//	無視フォルダ
					}),
					packageSettings	: new ABPackageMstTable( packageConfigFile ),
					buildProcess	: SimpleABBuildProcess<ABBuildData>.Empty,
					preProcess		: ABBuildPreProcess<ABBuildData>.Empty,
					postProcess		: new SaveBuildMapPostProcess( buildMapFile )
				)
			{
				
			}
		}
	}
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;

namespace Chipstar.Builder
{
	public interface IAssetBundleBuilder<TBuildResult> : IDisposable
		where TBuildResult : IABBuildResult
	{
		TBuildResult Build();
	}
	/// <summary>
	/// ビルド用クラス
	/// </summary>
	public class AssetBundleBuilder<TPackageData,TBuildData,TBuildResult>
		: IAssetBundleBuilder<TBuildResult>
		where TPackageData  : IABPackageData<TBuildData>
        where TBuildData    : IABBuildData
        where TBuildResult  : IABBuildResult
    {
		//============================================
		//	プロパティ
		//============================================
		private IABBuildConfig								Config			{ get; set; }
		private IABPackageSettings<TPackageData,TBuildData> PackageSettings	{ get; set; }
		private IPackageCalclater<TPackageData,TBuildData>	PackageCalclater		{ get; set; }

		private IABBuildFileFilter							FileFilter		{ get; set; }

		private IABBuildPreProcess<TBuildData>				PreProcess	{ get; set; }
		private IABBuildProcess<TBuildData,TBuildResult>	BuildProcess{ get; set; }
		private IABBuildPostProcess<TBuildData,TBuildResult>PostProcess { get; set; }
		//============================================
		//	関数
		//============================================

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public AssetBundleBuilder
		(
			IABBuildConfig									config,
			IABBuildFileFilter								fileFilter,
			IABPackageSettings<TPackageData, TBuildData>	packageSettings,
			IPackageCalclater<TPackageData, TBuildData>		packageCalclater,
			IABBuildProcess<TBuildData, TBuildResult>		buildProcess,
			IABBuildPreProcess<TBuildData>					preProcess,
			IABBuildPostProcess<TBuildData, TBuildResult>	postProcess
			)
		{
			Config			= config;
			PackageSettings = packageSettings;
			PackageCalclater= packageCalclater;
			FileFilter      = fileFilter;
			PreProcess		= preProcess;
			BuildProcess    = buildProcess;
			PostProcess     = postProcess;
		}

		/// <summary>
		/// 破棄処理
		/// </summary>
		public void Dispose()
		{
			Config			= null;
			PackageSettings = null;
			FileFilter      = null;
			PackageCalclater= null;
			PreProcess      = null;
			BuildProcess    = null;
			PostProcess     = null;
		}

        /// <summary>
        /// ビルド
        /// </summary>
        public virtual TBuildResult Build( )
        {
			//  ビルド対象アセットの絞り込み
			var projectAllAssets= AssetDatabase.GetAllAssetPaths();
			var buildAssets     = FileFilter.Refine( projectAllAssets );

			var packageGroup    = PackageSettings.CreatePackageList();

			//	ビルドマップの生成
			var assetBundleList = PackageCalclater.CreatePackageList( Config.RootFolder, buildAssets, packageGroup );

            //  事前処理
            PreProcess.OnProcess( Config, assetBundleList );
			
			//	ビルド実行
			var result = BuildProcess.Build( Config, assetBundleList );

			//	事後処理
            PostProcess.OnProcess( Config, result, assetBundleList );

            AssetDatabase.Refresh();

			return result;
		}

    }

	/// <summary>
	/// 
	/// </summary>
	public static partial class AssetBundleBuilder
	{
	}
}
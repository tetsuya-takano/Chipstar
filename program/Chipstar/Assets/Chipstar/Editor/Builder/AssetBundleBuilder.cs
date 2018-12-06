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
			IABBuildProcess<TBuildData, TBuildResult>		buildProcess,
			IABBuildPreProcess<TBuildData>					preProcess,
			IABBuildPostProcess<TBuildData, TBuildResult>	postProcess
			)
		{
			Config			= config;
			PackageSettings = packageSettings;
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
			var assetBundleList = CreateAssetBundlePackage( Config, buildAssets, packageGroup );

            //  事前処理
            PreProcess.OnProcess( assetBundleList );
			
			//	ビルド実行
			var result = BuildProcess.Build( Config, assetBundleList );

			//	事後処理
            PostProcess.OnProcess( Config, result, assetBundleList );

            AssetDatabase.Refresh();

			return result;
		}
		/// <summary>
		/// アセットバンドル生成結果配列の作成
		/// </summary>
		protected virtual IList<TBuildData> CreateAssetBundlePackage(
			IABBuildConfig		config				, 
			string[]			buildAssets         , 
			IList<TPackageData>	packageConfigList   
		)
		{
            var list            = new List<TBuildData>();
            var buildAssetTmp   = new List<string>( buildAssets );
			var packList		= packageConfigList.OrderBy( p => -p.Priority ).ToArray();

			using( var scope = new ProgressDialogScope( "Calclate Package", packList.Length ) )
			{
				for( var i = 0; i < packList.Length; i++ )
				{
					var pack    = packList[ i ];
					scope.Show( pack.Name, i );
					var bundles = Package( config, pack, ref buildAssetTmp );
					list.AddRange( bundles );
				}
			}
            return list;
		}

        protected virtual IList<TBuildData> Package( IABBuildConfig config, IABPackageData<TBuildData> pack, ref List<string> targetAssets )
        {
            //  パッケージ対象を抽出
            var packagedAssets = new List<string>( targetAssets.Where( p => pack.IsMatch( p ) ));
            //  パッケージ済みとして、残アセットから削除
            targetAssets.RemoveAll( p => packagedAssets.Contains( p ) );

            return pack.Build( config.RootFolder, packagedAssets );
        }
    }

	/// <summary>
	/// 
	/// </summary>
	public static partial class AssetBundleBuilder
	{
	}
}
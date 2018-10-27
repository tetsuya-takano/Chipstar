using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;

namespace Chipstar.Builder
{
    /// <summary>
    /// ビルド用クラス
    /// </summary>
	public class AssetBundleBuilder<TPackageData,TBuildData,TBuildResult> 
        where TPackageData  : IABPackageData<TBuildData>
        where TBuildData    : IABBuildData
        where TBuildResult  : IABBuildResult
    {
        /// <summary>
        /// ビルド
        /// </summary>
        public virtual void Build( 
            IABBuildConfig                                  config       ,
            IABBuildFileFilter                              fileFilter   ,
            IABPackageSettings   <TPackageData,TBuildData>  packageSettings,
            IABBuildProcess      <TBuildData,TBuildResult>  buildProcess ,
            IABBuildPreProcess   <TBuildData>               preProcess   ,
            IABBuildPostProcess  <TBuildData,TBuildResult>  postProcess  
        )
        {
            //  ビルド対象アセットの絞り込み
            var buildAssets     = FilteringBuildAssets( AssetDatabase.GetAllAssetPaths(), fileFilter );

            var packageGroup    = CreateBuildPackageGroup( packageSettings );

			//	ビルドマップの生成
			var assetBundleList = CreateAssetBundlePackage( config, buildAssets, packageGroup );

            //  事前処理
            preProcess.OnProcess( assetBundleList );
			
			//	ビルド実行
			var result = buildProcess.Build( config, assetBundleList );

			//	事後処理
            postProcess.OnProcess( config, result, assetBundleList );

            AssetDatabase.Refresh();
		}

        protected virtual IList<TPackageData> CreateBuildPackageGroup( IABPackageSettings<TPackageData, TBuildData> packageSettings )
        {
            return packageSettings.CreatePackageList();
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual string[] FilteringBuildAssets( string[] allAssetPaths, IABBuildFileFilter filter )
        {
            return filter.Refine( allAssetPaths );
        }

		/// <summary>
		/// アセットバンドル生成結果配列の作成
		/// </summary>
		protected virtual IList<TBuildData> CreateAssetBundlePackage(
			IABBuildConfig		                config              , 
			string[]					        buildAssets         , 
			IList<TPackageData>	packageConfigList   
		)
		{
            var list            = new List<TBuildData>();
            var buildAssetTmp   = new List<string>( buildAssets );
            foreach( var pack in packageConfigList.OrderBy( p => -p.Priority ) )
            {
                var bundles = Package( pack, ref buildAssetTmp );
                list.AddRange( bundles );
            }
            return list;
		}

        protected virtual IList<TBuildData> Package( IABPackageData<TBuildData> pack, ref List<string> targetAssets )
        {
            //  パッケージ対象を抽出
            var packagedAssets = new List<string>( targetAssets.Where( p => pack.IsMatch( p ) ));
            //  パッケージ済みとして、残アセットから削除
            targetAssets.RemoveAll( p => packagedAssets.Contains( p ) );

            return pack.Build( packagedAssets );
        }
    }
}
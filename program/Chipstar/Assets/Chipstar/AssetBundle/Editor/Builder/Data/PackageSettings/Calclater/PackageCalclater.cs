using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Chipstar.Builder
{
	public interface IPackageCalclater<TPackageData, TBuildData>
		where TPackageData : IABPackageData<TBuildData>
		where TBuildData : IABBuildData
	{
		IList<TBuildData> CreatePackageList( string rootFolder, string[] buildAssets, IList<TPackageData> packageConfigList );
	}
	/// <summary>
	/// パッケージの計算
	/// </summary>
	public class PackageCalclater<TPackageData, TBuildData> : IPackageCalclater<TPackageData, TBuildData>
		where TPackageData : IABPackageData<TBuildData>
		where TBuildData	: IABBuildData
	{
		/// <summary>
		/// アセットバンドル生成結果配列の作成
		/// </summary>
		public IList<TBuildData> CreatePackageList(
			string	 rootFolder,
			string[] buildAssets,
			IList<TPackageData> packageConfigList
		)
		{
			var packageTable    = new Dictionary<string,TBuildData>();
			var buildAssetTmp   = new List<string>( buildAssets );
			var packList        = packageConfigList.OrderBy( p => -p.Priority ).ToArray();

			using( var scope = new ProgressDialogScope( "Calclate Package", packList.Length ) )
			{
				for( var i = 0; i < packList.Length; i++ )
				{
					var pack    = packList[ i ];
					scope.Show( pack.Name, i );
					var bundles = Package( rootFolder, pack, ref buildAssetTmp );
					
					foreach( var b in bundles )
					{
						if( packageTable.ContainsKey( b.ABName ) )
						{
							packageTable[b.ABName].Merge( b );
						}
						else
						{
							packageTable.Add( b.ABName, b );
						}
					}
				}
			}
			return packageTable.Values.ToArray();
		}

		protected virtual IList<TBuildData> Package( string rootFolder, IABPackageData<TBuildData> pack, ref List<string> targetAssets )
		{
			//  パッケージ対象を抽出
			var packagedAssets = targetAssets
									.Where(p => pack.IsMatch(rootFolder, p))
									.ToList();

			//  パッケージ済みとして、残アセットから削除
			targetAssets.RemoveAll( p => packagedAssets.Contains( p ) );

			return pack.Build( rootFolder, packagedAssets );
		}
	}
}
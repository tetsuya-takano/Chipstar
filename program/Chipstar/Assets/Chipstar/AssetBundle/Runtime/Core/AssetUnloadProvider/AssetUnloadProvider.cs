using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Chipstar.Downloads
{
	/// <summary>
	/// 破棄管理
	/// </summary>
	public interface IAssetUnloadProvider : IDisposable
	{
		IEnumerator UnloadUnusedAssets	();
		IEnumerator ForceReleaseAll		();
	}
	/// <summary>
	/// 
	/// </summary>
	public class AssetUnloadProvider<T> 
		:	IAssetUnloadProvider
			where T : IRuntimeBundleData<T>
	{
		//========================================
		//	プロパティ
		//========================================
		private ILoadDatabase<T> Database { get; set; }

		//========================================
		//	関数
		//========================================

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public AssetUnloadProvider( ILoadDatabase<T> database )
		{
			Database = database;
		}

		/// <summary>
		/// 破棄
		/// </summary>
		public void Dispose()
		{
			Database = null;
		}

		/// <summary>
		/// 未使用のモノを破棄
		/// </summary>
		public IEnumerator UnloadUnusedAssets()
		{
			if( Database != null )
			{
				//	参照の無いやつを取得
				var freeList = Database
								.BundleList
								.Where(c => c.IsFree && c.IsOnMemory )
								.ToArray();
				//	解放
				foreach (var bundle in freeList)
				{
					bundle.Unload();
				}
				ChipstarLog.Log_DisposeUnused( freeList );
			}
			
			//	Resourcesの未使用を解放
			yield return null;
			yield return Resources.UnloadUnusedAssets();

			yield return null;
			GC.Collect( 0 );
		}

		/// <summary>
		/// 強制解放
		/// </summary>
		public IEnumerator ForceReleaseAll()
		{
			//	すべての参照を解放
			foreach( var bundle in Database.BundleList )
			{
				bundle.ClearRef();
				bundle.Unload();
			}
			yield return null;
			AssetBundle.UnloadAllAssetBundles( true );
		}
	}
}
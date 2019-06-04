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
		void		AddRef				( string assetPath );
		void		ReleaseRef			( string assetPath );
		IDisposable CreateRefCounter	( string assetPath );
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
		/// 参照の加算
		/// </summary>
		public void AddRef( string assetPath )
		{
			if( !Database.Contains( assetPath ) )
			{
				return;
			}
			var data = Database.GetAssetData( assetPath );

			data?.AddRef();
		}

		/// <summary>
		/// 参照の減算
		/// </summary>
		public void ReleaseRef( string assetPath )
		{
			if( Database == null)
			{
				return;
			}
			if( !Database.Contains( assetPath ) )
			{
				return;
			}
			var data = Database.GetAssetData( assetPath );
			data?.ReleaseRef();
		}

		/// <summary>
		/// 参照カウンタ用のインスタンスを追加
		/// </summary>
		public IDisposable CreateRefCounter( string assetPath )
		{
			if( !Database.Contains( assetPath ) )
			{
				return EmptyReference.Default;
			}
			var data = Database.GetAssetData( assetPath );
			return new RefCalclater( data );
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
								.Where(c => c.IsFree )
								.ToArray();
				//	解放
				foreach (var bundle in freeList)
				{
					bundle.Unload();
				}
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
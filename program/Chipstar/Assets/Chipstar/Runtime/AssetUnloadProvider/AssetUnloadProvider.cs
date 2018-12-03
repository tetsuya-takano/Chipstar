using System;
using System.Collections;
using System.Collections.Generic;
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
		IEnumerator ForceUnloadAll		();
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

			data.BundleData.AddRef();
		}

		/// <summary>
		/// 参照の減算
		/// </summary>
		public void ReleaseRef( string assetPath )
		{
			if( !Database.Contains( assetPath ) )
			{
				return;
			}
			var data = Database.GetAssetData( assetPath );
			data.BundleData.ReleaseRef();
		}

		/// <summary>
		/// 参照カウンタ用のインスタンスを追加
		/// </summary>
		public IDisposable CreateRefCounter( string assetPath )
		{
			var data = Database.GetAssetData( assetPath );
			if( data == null )
			{
				return null;
			}
			return new RefCalclater( data.BundleData );
		}

		/// <summary>
		/// 未使用のモノを破棄
		/// </summary>
		public IEnumerator UnloadUnusedAssets()
		{
			//	アセットバンドル解放
			foreach( var bundle in Database.BundleList)
			{
				if( bundle.IsFree )
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
		public IEnumerator ForceUnloadAll()
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
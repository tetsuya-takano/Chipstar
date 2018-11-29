using Chipstar.Downloads;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chipstar.Downloads
{
	/// <summary>
	/// アセットバンドルからロードするやつ
	/// </summary>
	public class AssetBundleLoadFactory<TRuntimeBundleData> 
			: IAssetLoadFactory
		where TRuntimeBundleData : IRuntimeBundleData<TRuntimeBundleData>
	{
		//======================================
		//	プロパティ
		//======================================
		private ILoadDatabase<TRuntimeBundleData> Database { get; set; }

		//======================================
		//	関数
		//======================================

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public AssetBundleLoadFactory( ILoadDatabase<TRuntimeBundleData> database)
		{
			Database = database;
		}

		/// <summary>
		/// 判定
		/// </summary>
		public bool CanLoad( string path )
		{
			return Database.Contains( path );
		}

		/// <summary>
		/// 作成
		/// </summary>
		public ILoadOperation<T> Create<T>( string path ) where T : UnityEngine.Object
		{
			var data = Database.Find( path );
			return new AssetBundleLoadOperation<T>( 
							data.LoadAsync<T>( ), 
							Database.AddReference( data.BundleData ) 
						);
		}

		/// <summary>
		/// 破棄処理
		/// </summary>
		public void Dispose()
		{
			Database = null;
		}
	}
}
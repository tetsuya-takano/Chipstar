using Chipstar.Downloads;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Chipstar.Downloads
{
	/// <summary>
	/// シーン読み込み機能を生成
	/// </summary>
	public sealed class SceneLoadFactory<TRuntimeBundleData>
		: ISceneLoadFactory
		where TRuntimeBundleData : IRuntimeBundleData<TRuntimeBundleData>
	{
		//================================
		//	プロパティ
		//================================
		private ILoadDatabase<TRuntimeBundleData> Database;

		//================================
		//	関数
		//================================
		
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public SceneLoadFactory( ILoadDatabase<TRuntimeBundleData> database )
		{
			Database = database;
		}

		/// <summary>
		/// 破棄処理
		/// </summary>
		public void Dispose()
		{
			Database = null;
		}

		/// <summary>
		/// 判定
		/// </summary>
		public bool CanLoad( string path )
		{
			return Database.Contains( path );
		}

		/// <summary>
		/// シーンロード
		/// </summary>
		public ISceneLoadOperater Create( string path, LoadSceneMode mode )
		{
			var data = Database.GetAssetData( path );
			return new AssetBundleSceneLoadOperation<TRuntimeBundleData>(data, mode);
		}
	}
}
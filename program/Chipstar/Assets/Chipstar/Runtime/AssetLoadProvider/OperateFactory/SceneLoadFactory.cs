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
			if( !Database.Contains( path ) )
			{
				return false;
			}
			var data = Database.GetAssetData( path );

			return data.BundleData.IsScene;
		}

		/// <summary>
		/// シーンロード
		/// </summary>
		public ISceneLoadOperation LoadLevel( string path )
		{
			return new SceneLoadOperation( SceneManager.LoadSceneAsync( path ) );
		}
		/// <summary>
		/// 加算ロード
		/// </summary>
		public ISceneLoadOperation LoadLevelAdditive( string path )
		{
			return new SceneLoadOperation( SceneManager.LoadSceneAsync( path, LoadSceneMode.Additive ) );
		}
	}
}
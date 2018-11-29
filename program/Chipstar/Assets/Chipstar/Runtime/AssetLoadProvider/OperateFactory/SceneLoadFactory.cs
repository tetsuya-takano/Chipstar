using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Chipstar.AssetLoad
{
	/// <summary>
	/// シーン読み込み機能を生成
	/// </summary>
	public sealed class SceneLoadFactory : ISceneLoadFactory
	{
		/// <summary>
		/// 破棄処理
		/// </summary>
		public void Dispose()
		{
		}

		/// <summary>
		/// 判定
		/// </summary>
		public bool CanLoad( string path )
		{
			var scene = SceneManager.GetSceneByPath( path );
			if( !scene.IsValid() )
			{
				return false;
			}
			return true;
		}

		/// <summary>
		/// シーンロード
		/// </summary>
		public AsyncOperation LoadLevel( string path )
		{
			var scene = SceneManager.GetSceneByPath( path );
			if( !scene.IsValid() )
			{
				return null;
			}
			return SceneManager.LoadSceneAsync( scene.name );
		}
		/// <summary>
		/// 加算ロード
		/// </summary>
		public AsyncOperation LoadLevelAdditive( string path )
		{
			var scene = SceneManager.GetSceneByPath( path );
			if( !scene.IsValid() )
			{
				return null;
			}
			return SceneManager.LoadSceneAsync( scene.name, LoadSceneMode.Additive );
		}
	}
}
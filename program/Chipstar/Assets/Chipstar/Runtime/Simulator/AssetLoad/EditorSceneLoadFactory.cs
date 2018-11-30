using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
namespace Chipstar.Downloads
{
	/// <summary>
	/// エディタでのシーン読み込み処理
	/// </summary>
	public sealed class EditorSceneLoadFactory : ISceneLoadFactory
	{
		//======================================
		//	変数
		//======================================

		//======================================
		//	関数
		//======================================

		/// <summary>
		/// 判定
		/// </summary>
		public bool CanLoad( string path )
		{
			return true;
		}

		/// <summary>
		/// 破棄
		/// </summary>
		public void Dispose() { }

		/// <summary>
		/// 遷移
		/// </summary>
		public ISceneLoadOperation LoadLevel( string path )
		{
			var operation = EditorApplication.LoadLevelAsyncInPlayMode( path );
			return new SceneLoadOperation( operation );
		}

		/// <summary>
		/// 加算
		/// </summary>
		public ISceneLoadOperation LoadLevelAdditive( string path )
		{
			var operation = EditorApplication.LoadLevelAdditiveAsyncInPlayMode( path );
			return new SceneLoadOperation( operation );
		}
	}
}
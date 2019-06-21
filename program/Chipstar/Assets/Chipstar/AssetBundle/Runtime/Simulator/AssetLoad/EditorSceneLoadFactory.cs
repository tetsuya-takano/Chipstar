#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace Chipstar.Downloads
{
	/// <summary>
	/// エディタでのシーン読み込み処理
	/// </summary>
	public sealed class EditorSceneLoadFactory : ISceneLoadFactory
	{
		public int Priority => 99;

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
			return SceneUtility.GetBuildIndexByScenePath( path ) == -1;
		}

		/// <summary>
		/// 破棄
		/// </summary>
		public void Dispose() { }

		/// <summary>
		/// 遷移
		/// </summary>
		public ISceneLoadOperater Create( string path, LoadSceneMode mode )
		{
			return new EditorSceneLoadOperation( path, mode );
		}
	}
}
#endif
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Chipstar.Downloads
{
	/// <summary>
	/// 内包シーンの読み込みタスク生成
	/// </summary>
	public sealed class BuiltInSceneLoadFactory : ISceneLoadFactory
	{
		//======================================
		//	プロパティ
		//======================================
		public int Priority { get; }

		//======================================
		//	関数
		//======================================

		public BuiltInSceneLoadFactory(int priority) { Priority = priority; }

		/// <summary>
		/// 破棄
		/// </summary>
		public void Dispose()
		{
		}

		/// <summary>
		/// 判定処理
		/// </summary>
		public bool CanLoad( string path )
		{
			var index = SceneUtility.GetBuildIndexByScenePath( path );
			return index != -1;
		}

		/// <summary>
		/// シーンロード
		/// </summary>
		public ISceneLoadOperater Create( string path, LoadSceneMode mode )
		{
			return new BuiltInSceneLoadOperation( path, mode );
		}

	}
}
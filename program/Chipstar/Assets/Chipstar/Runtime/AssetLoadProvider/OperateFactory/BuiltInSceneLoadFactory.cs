﻿using System;
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
		//	関数
		//======================================

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
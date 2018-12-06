﻿#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Chipstar.Downloads
{
	/// <summary>
	/// エディタでのアセットアクセスを作成
	/// </summary>
	public sealed class EditorLoadAssetFactory : IAssetLoadFactory
	{
		//============================
		//	変数
		//============================
		private string m_folderPrefix = null;

		//============================
		//	関数
		//============================

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public EditorLoadAssetFactory(string prefex )
		{
			m_folderPrefix = prefex;
		}
		/// <summary>
		/// 判定
		/// </summary>
		public bool CanLoad( string path )
		{
			var isExist = File.Exists( Path.Combine( m_folderPrefix, path ) );
			return isExist;
		}

		/// <summary>
		/// 作成
		/// </summary>
		public IAssetLoadOperation<T> Create<T>( string path ) where T : UnityEngine.Object
		{
			var fullPath = Path.Combine( m_folderPrefix, path );
			return new EditorLoadAssetOperation<T>( fullPath );
		}

		/// <summary>
		/// 破棄
		/// </summary>
		public void Dispose()
		{
		}
	}
}
#endif
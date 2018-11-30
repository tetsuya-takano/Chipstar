#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chipstar.Downloads
{
	/// <summary>
	/// エディタでのアセットアクセスを作成
	/// </summary>
	public sealed class EditorLoadAssetFactory : IAssetLoadFactory
	{
		/// <summary>
		/// 判定
		/// </summary>
		public bool CanLoad( string path )
		{
			return true;
		}

		/// <summary>
		/// 作成
		/// </summary>
		public IAssetLoadOperation<T> Create<T>( string path ) where T : UnityEngine.Object
		{
			return new EditorLoadAssetOperation<T>( path );
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
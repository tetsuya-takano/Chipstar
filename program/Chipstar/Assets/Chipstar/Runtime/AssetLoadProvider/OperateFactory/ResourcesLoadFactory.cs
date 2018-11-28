using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
namespace Chipstar.AssetLoad
{
	/// <summary>
	/// Resourcesロードをするヤツ
	/// </summary>
	public sealed class ResourcesLoadFactory : IAssetLoadFactory
	{
		/// <summary>
		/// 取得可能かどうか
		/// </summary>
		public bool CanLoad( string path )
		{
			//	拡張子指定ナシはたぶんResources
			return !Path.HasExtension( path );
		}

		/// <summary>
		/// リクエスト作成
		/// </summary>
		public ILoadOperation<T> Create<T>( string path ) where T : UnityEngine.Object
		{
			return new ResourcesLoadOperation<T>( Resources.LoadAsync<T>( path ) );
		}

		/// <summary>
		/// 破棄処理
		/// </summary>
		public void Dispose()
		{

		}
	}
}
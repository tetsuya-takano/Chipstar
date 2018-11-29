using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
namespace Chipstar.Downloads
{
	/// <summary>
	/// Resourcesロードをするヤツ
	/// </summary>
	public sealed class ResourcesLoadFactory : IAssetLoadFactory
	{
		//==============================
		//	const
		//==============================
		private const           string  PATTERN = "(/|\\s)Resources/(.*)";
		private static readonly Regex   m_regex = new Regex( PATTERN );

		//==============================
		//	変数
		//==============================

		//==============================
		//	関数
		//==============================

		/// <summary>
		/// 取得可能かどうか
		/// </summary>
		public bool CanLoad( string path )
		{
			//	拡張子指定ナシはたぶんResources
			return m_regex.IsMatch( path );
		}

		/// <summary>
		/// リクエスト作成
		/// </summary>
		public ILoadOperation<T> Create<T>( string path ) where T : UnityEngine.Object
		{
			var match = m_regex.Match( path );
			if( !match.Success )
			{
				return null;
			}

			//	Resources以下を拾う
			var accessKey = match.Groups[2].Value;
			if( Path.HasExtension( accessKey ))
			{
				//	拡張子は削る
				accessKey = accessKey.Replace( Path.GetExtension( accessKey ), string.Empty );
			}
			return new ResourcesLoadOperation<T>( Resources.LoadAsync<T>( accessKey ) );
		}

		/// <summary>
		/// 破棄処理
		/// </summary>
		public void Dispose()
		{

		}
	}
}
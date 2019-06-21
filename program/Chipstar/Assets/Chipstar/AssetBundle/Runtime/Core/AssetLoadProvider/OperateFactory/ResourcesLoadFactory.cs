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
		//	プロパティ
		//==============================
		public int Priority { get; }

		//==============================
		//	関数
		//==============================
		public ResourcesLoadFactory(int priority) { Priority = priority; }

		/// <summary>
		/// 取得可能かどうか
		/// </summary>
		public bool CanLoad( string path )
		{
			//	Resourcesは素通し
			return true;
		}

		/// <summary>
		/// リクエスト作成
		/// </summary>
		public IAssetLoadOperater<T> Create<T>( string path ) where T : UnityEngine.Object
		{
			var key = path;
			if( !Path.HasExtension( key ) )
			{
				//	拡張子の無いリクエストは警告を出す
				ChipstarLog.Log_WarningNotHasExtensions( key );
			}
			else
			{
				//	拡張子があったら削る
				key = path.Replace( Path.GetExtension( key ), string.Empty );
			}
			var match = m_regex.Match( key );
			if( !match.Success )
			{
				return new ResourcesLoadOperation<T>( key );
			}

			//	Resources以下を拾う
			var accessKey = match.Groups[2].Value;
			return new ResourcesLoadOperation<T>( key );
		}

		/// <summary>
		/// 破棄処理
		/// </summary>
		public void Dispose()
		{

		}
	}
}
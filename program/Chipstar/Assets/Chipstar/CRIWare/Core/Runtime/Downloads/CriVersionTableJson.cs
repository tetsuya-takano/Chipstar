using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace Chipstar.Downloads.CriWare
{
	/// <summary>
	/// バージョン情報を書き込んでおくテーブル
	/// </summary>
	[Serializable]
	public sealed class CriVersionTableJson : IDisposable
	{
		//==================================
		//	class
		//==================================
		[Serializable]
		private sealed class Data
		{
			public string key  = string.Empty;
			public string hash = string.Empty;
		}

		//==================================
		//	変数
		//==================================
		[SerializeField] private List<Data> m_list			= new List<Data>();

		//==================================
		//	関数
		//==================================

		/// <summary>
		/// ローカルのファイルを取得
		/// </summary>
		public static CriVersionTableJson ReadLocal( string path, Encoding encoding )
		{
			var isExist     = File.Exists( path );
			if( !isExist )
			{
				//なければ空データ
				return new CriVersionTableJson();
			}
			var contents    = File.ReadAllText( path, encoding );
			return JsonUtility.FromJson<CriVersionTableJson>( contents );
		}

		/// <summary>
		/// 破棄
		/// </summary>
		public void Dispose()
		{

		}

		/// <summary>
		/// 追加 / 差し替え
		/// </summary>
		public void AddNew( string key, string hash )
		{
			var data = Find( key );
			ChipstarLog.AssertNotNull(data, $"Only Add New : {key}");
			m_list.Add( new Data
			{
				key = key,
				hash= hash
			} );
		}

		/// <summary>
		/// 相手側の値で上書き
		/// </summary>
		public void Replace( string key, string hash )
		{
			AddOrReplace( key, hash );
		}

		/// <summary>
		/// 追加か書き込み
		/// </summary>
		private void AddOrReplace( string key, string hash )
		{
			var data = Find( key );
			if( data == null)
			{
				m_list.Add( new Data
				{
					key = key,
					hash= hash
				} );
				return;
			}
			data.key	= key;
			data.hash	= hash;
		}

		/// <summary>
		/// ファイル保存
		/// </summary>
		public void WriteFile( string path, Encoding encoding )
		{
			var dirPath = Path.GetDirectoryName( path );
			if( !Directory.Exists( dirPath ) )
			{
				Directory.CreateDirectory( dirPath );
			}
			var json = JsonUtility.ToJson( this, true );
			File.WriteAllText( path, json, encoding );
		}

		/// <summary>
		/// 一致判定
		/// </summary>
		public bool IsSameVersion( string key, string hash )
		{
			var data = Find( key );
			if( data == null )
			{
				//	キーが無いなら一致しない
				return false;
			}

			if( data.hash != hash)
			{
				//	ハッシュが一致したら同じ
				return false;
			}

			return true;
		}
		public string GetVersion(string key)
		{
			return Find(key)?.hash ?? string.Empty;
		}

		/// <summary>
		/// 検索
		/// </summary>
		private Data Find( string key )
		{
			return m_list.Find( c => c.key == key );
		}

		/// <summary>
		/// デバッグ出力用
		/// </summary>
		public override string ToString()
		{
			var builder = new StringBuilder();

			foreach( var d in m_list)
			{
				builder
					.Append( d.key )
					.Append("/")
					.Append( d.hash )
					.AppendLine();
			}
			return builder.ToString();
		}
		
	}
}
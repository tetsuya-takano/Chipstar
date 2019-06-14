using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.IO;
using System.Linq;

namespace Chipstar.Builder
{
	/// <summary>
	/// ファイル差分情報のデータベースファイルを作成するクラス
	/// </summary>
	public sealed class FileHashDatabaseBuilder
	{
		public sealed class FileHashData
		{
			public string Key			{ get; set; }
			public string Hash			{ get; set; }
			public FileInfo FileInfo	{ get; set; }
		}
		//================================
		//	変数
		//================================
		private string				m_rootFolder		= string.Empty;
		private Regex[]             m_regexes           = null;

		private IFileHashCalclater  m_hashCalclater     = null;

		//================================
		//	関数
		//================================

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public FileHashDatabaseBuilder( string folder, string[] pattern )
		{
			m_rootFolder    = folder.ToConvertDelimiter();
			m_hashCalclater = new FileHashCalclater<SHA1>( SHA1.Create() );
			m_regexes = new Regex[pattern.Length];
			for( int i = 0; i < pattern.Length; i++ )
			{
				m_regexes[i] = new Regex( pattern[i] );
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public Dictionary<string, FileHashData> Build()
		{
			if( !Directory.Exists( m_rootFolder ) )
			{
				return new Dictionary<string, FileHashData>();
			}
			//	対象ファイルのフィルタリング
			var allFiles		= Directory.GetFiles( m_rootFolder, "*", SearchOption.AllDirectories );
			var filterdList		= allFiles
									.Where( path => m_regexes.Any( r => r.IsMatch( path ) ))
									.Select( p => p.ToConvertDelimiter() )
									.ToArray();
			//	テーブル
			var list = new Dictionary<string,FileHashData>();
			using( var dialog = new ProgressDialogScope( "Calc FileHash", filterdList.Length ) )
			{
				for( int i = 0; i < filterdList.Length; i++ )
				{
					var file = filterdList[ i ];
					var path = file.Replace( "\\","/" );
					var key  = path.Replace( m_rootFolder, string.Empty );
					var info = new FileInfo( path );
					var hash = m_hashCalclater.Calclate( info );
					list.Add( key, new FileHashData
					{
						Key		 = key,
						Hash	 = hash,
						FileInfo = info
					} );
					dialog.Show( key + " == " + hash, i );
				}
			}
			return list;
		}
	}
}
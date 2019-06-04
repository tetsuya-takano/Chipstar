using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Chipstar.Downloads
{
	/// <summary>
	/// パス / Hash ペアデータ
	/// </summary>
	[Serializable]
	public sealed class FileVersionData
	{
		[SerializeField] private string m_path    = string.Empty;
		[SerializeField] private string m_version = string.Empty;

		public string Path { get { return m_path; } }
		public string Version { get { return m_version; } }

		public static FileVersionData Parse(string line)
		{
			//	「", \"」
			var targets = new string[]
			{
				" ",
				",",
				"\\",
				"\"",
			};
			string tmp = line;
			foreach (var s in targets)
			{
				tmp = tmp.Replace( s, string.Empty );
			}
			var args = tmp.Split(':');
			var path    = args.ElementAtOrDefault( 0 );
			var version = args.ElementAtOrDefault( 1 );

			return new FileVersionData
			{
				m_path = path,
				m_version = version
			};
		}
	}
	/// <summary>
	/// パスとバージョンHashの統合テーブル
	/// コレをDLしてくる
	/// </summary>
	[Serializable]
	public sealed class FileVersionManifest
	{
		//==================================
		//	SerializeField
		//==================================
		[SerializeField] private List<FileVersionData> m_list = new List<FileVersionData>();
		//==================================
		//	プロパティ
		//==================================
		public IReadOnlyList<FileVersionData> List { get { return m_list; } }

		/// <summary>
		/// 変換
		/// </summary>
		public static FileVersionManifest FromJson(string json)
		{
			//	JsonUtilityで取れない形式なので
			//	自前でパースする
			if (json == null)
			{
				return null;
			}
			var obj = new FileVersionManifest();
			//	解析
			using( var reader = new StringReader( json ))
			{
				var list = new List<FileVersionData>();
				while( reader.Peek() > -1)
				{
					var line = reader.ReadLine();
					if (line.Contains("{")) { continue; } // 開始
					if (line.Contains("}") || line == null ) { break; } // 終了

					// 「"  \"AAA/BBB/CCC.ddd\": \"xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx\","」
					var data = FileVersionData.Parse( line );
					list.Add( data );
				}
				obj.m_list = list;
			}
			return obj;
		}
	}

	/// <summary>
	/// 内容物別にバージョンデータを振り分けて使うテーブル
	/// </summary>
	public sealed class FileVersionTable
	{
		//==================================
		//	変数
		//==================================
		private string m_prefex = string.Empty;
		private Dictionary<string, string> m_table = new Dictionary<string, string>();

		//==================================
		//	関数
		//==================================

		/// <summary>
		/// 
		/// </summary>
		public FileVersionTable(string prefix, IEnumerable<FileVersionData> sourceList)
		{
			m_prefex = prefix;
			//	パスから prefix をちぎったテーブル作成する
			m_table = sourceList
				.Where(c => c.Path.StartsWith( prefix ) )
				.ToDictionary(
					keySelector: c => 
					{
						var p = c.Path.Replace(prefix, string.Empty);
						if ( p[0] == '/')
						{
							p = p.Substring( 1 );
						}
						return p;
					},
					elementSelector : c => c.Version
				);
		}

		/// <summary>
		/// バージョンの取得
		/// </summary>
		public string Get( string key )
		{
			string value = string.Empty;
			if (m_table.TryGetValue(key, out value))
			{
				return value;
			}
			return value;
		}

		public override string ToString()
		{
			var builder = new StringBuilder();

			builder.AppendLine(m_prefex);
			foreach ( var item in m_table)
			{
				builder.AppendLine($"{item.Key} , {item.Value}");
			}
			return builder.ToString();
		}
	}
}
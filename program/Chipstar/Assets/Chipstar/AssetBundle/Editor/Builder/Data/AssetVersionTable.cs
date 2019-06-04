using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

namespace Chipstar.Builder
{
	/// <summary>
	/// アセットバージョンのみを管理するテーブルクラス
	/// </summary>
	[Serializable]
	public sealed class AssetVersionTable
	{
		//=====================================
		//	const
		//=====================================
		private static readonly Encoding Encode = new UTF8Encoding( false );

		//=====================================
		//	変数
		//=====================================
		[SerializeField] private List<string> m_HistoryList = new List<string>();
		[SerializeField] private string		  m_NewestVersion		= string.Empty;

		//=====================================
		//	プロパティ
		//=====================================
		public string				NewestVersion { get { return m_NewestVersion; } }
		public IEnumerable<string>	HistoryList		{ get { return m_HistoryList; } }
		//=====================================
		//	関数
		//=====================================

		/// <summary>
		/// 更新
		/// </summary>
		public void Push()
		{
			if( !string.IsNullOrEmpty( m_NewestVersion ) )
			{
				//	旧バージョンを履歴へ
				m_HistoryList.Add( m_NewestVersion );
			}
			//	新バージョンを登録
			m_NewestVersion = CalcNewVersion();
		}
		private string CalcNewVersion()
		{
			var guid = Guid.NewGuid();
			return guid.ToString().Replace( "-", string.Empty );
		}

		/// <summary>
		/// リセット
		/// </summary>
		public void Clear()
		{
			m_HistoryList.Clear();
		}

		//=====================================
		//	static
		//=====================================

		/// <summary>
		/// 取得
		/// </summary>
		public static AssetVersionTable Read( string outputPath )
		{
			if( !File.Exists( outputPath ))
			{
				return new AssetVersionTable();
			}
			var json = File.ReadAllText( outputPath, Encode );
			return JsonUtility.FromJson<AssetVersionTable>( json );
		}

		/// <summary>
		/// 書き込み
		/// </summary>
		public static bool Write( string outputPath, AssetVersionTable table )
		{
			if( table == null)
			{
				return false;
			}
			var json = JsonUtility.ToJson( table, true );
			File.WriteAllText( outputPath, json, Encode );


			return true;
		}
	}
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Chipstar.Downloads.CriWare
{
	public interface ISoundFileDatabase : IDisposable, IEnumerable<ISoundFileData>
	{
		bool			Contains( string cueSheetName );
		ISoundFileData	Find	( string cueSheetName );
	}

	public interface ISoundFileData
	{
		string CueSheetName { get; }
		string AwbPath { get; }
		string AwbHash { get; }
		long   AwbSize { get; }

		string AcbPath { get; }
		string AcbHash { get; }
		long   AcbSize { get; }

		string AssetVersion { get; }

		bool HasAwb();
		bool IsInclude();
	}

	/// <summary>
	/// サウンドファイルのテーブル
	/// サウンドのバージョンチェック用
	/// </summary>
	[Serializable]
	public sealed class SoundFileDatabase : ISoundFileDatabase
	{
		//=============================
		//	const
		//=============================
		private const string AWB_FORMAT = "{0}.awb";
		private const string ACB_FORMAT = "{0}.acb";
		private static readonly Encoding Encode = new UTF8Encoding( false );
		//=============================
		//	class
		//=============================
		[Serializable]
		private class SoundFileData : ISoundFileData
		{
			public string CueSheetName; //	キューシート名
			public string DirPath;		//	階層
			public string AcbHash;      //	Acbファイルのハッシュ値
			public string AwbHash;      //	Awbファイルのハッシュ値
			public long   AcbSize;      //	Acbファイル容量
			public long   AwbSize;      //	Acbファイル容量
			public string AssetVersion; //	アセットバージョン

			public SoundFileData( string dirPath, ISoundFileData data )
			{
				this.CueSheetName   = data.CueSheetName;
				this.DirPath		= dirPath;

				this.AcbHash		= data.AcbHash;
				this.AcbSize        = data.AcbSize;

				this.AwbHash		= data.AwbHash;
				this.AwbSize        = data.AwbSize;
				this.AssetVersion   = data.AssetVersion;
			}

			string ISoundFileData.CueSheetName { get { return CueSheetName; } }

			string ISoundFileData.AcbPath { get { return Path.Combine( DirPath, string.Format( ACB_FORMAT, CueSheetName ) ).ToConvertDelimiter(); } }
			string ISoundFileData.AwbPath { get { return Path.Combine( DirPath, string.Format( AWB_FORMAT, CueSheetName ) ).ToConvertDelimiter(); } }

			string ISoundFileData.AwbHash { get { return AwbHash; } }
			string ISoundFileData.AcbHash { get { return AcbHash; } }

			long ISoundFileData.AwbSize { get { return AwbSize; } }
			long ISoundFileData.AcbSize { get { return AcbSize; } }

			string ISoundFileData.AssetVersion { get { return AssetVersion; } }

			bool ISoundFileData.HasAwb()
			{
				return !string.IsNullOrEmpty( AwbHash );
			}

			bool ISoundFileData.IsInclude()
			{
				//	DLファイルなのでfalse固定
				return false;
			}
		}
		[SerializeField] private List<SoundFileData> m_list = new List<SoundFileData>();

		//=============================
		//	関数
		//=============================

		public static SoundFileDatabase Read( string saveFilePath )
		{
			if( !File.Exists( saveFilePath ) )
			{
				return new SoundFileDatabase();
			}
			var json = File.ReadAllText( saveFilePath, Encode );
			return JsonUtility.FromJson<SoundFileDatabase>( json );
		}

		public static bool Write( string saveFilePath, SoundFileDatabase table )
		{
			var contents    = JsonUtility.ToJson( table, true );
			if( string.IsNullOrEmpty( contents ))
			{
				return false;
			}
			//  書き込み
			File.WriteAllText( saveFilePath, contents, Encode );

			return true;
		}
		/// <summary>
		/// 破棄
		/// </summary>
		public void Dispose()
		{

		}

		/// <summary>
		/// 所持判定
		/// </summary>
		public bool Contains( string cueSheetName )
		{
			return m_list.Any( c => c.CueSheetName == cueSheetName );
		}

		/// <summary>
		/// 取得
		/// </summary>
		public ISoundFileData Find( string cueSheetName )
		{
			return m_list.Find( c => c.CueSheetName == cueSheetName );
		}

		/// <summary>
		/// 
		/// </summary>
		public void Add( string dirPath, ISoundFileData data )
		{
			m_list.Add( new SoundFileData( dirPath, data ) );
		}

		IEnumerator<ISoundFileData> IEnumerable<ISoundFileData>.GetEnumerator()
		{
			foreach( var d in m_list)
			{
				yield return (d as ISoundFileData);
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return m_list.GetEnumerator();
		}
	}

	public sealed class LocalSoundDatabase : ISoundFileDatabase
	{
		/// <summary>
		/// 内包サウンド情報クラス
		/// </summary>
		private sealed class LocalSoundFile : ISoundFileData
		{
			public string AcbPath		{ get; private set; }
			public string AwbPath		{ get; private set; }
			public string CueSheetName	{ get;  private set; }

			public bool HasAwb()
			{
				return !string.IsNullOrEmpty( AwbPath );
			}
			public bool IsInclude()
			{
				//	内包データなのでtrue固定
				return true;
			}
			//======== 内包データなので考えなくてよい
			public string	AcbHash { get { return string.Empty; } }
			public long		AcbSize { get { return 0; } }
			public string	AwbHash { get { return string.Empty; } }
			public long		AwbSize { get { return 0; } }
			public string	AssetVersion { get { return string.Empty; } }

			public LocalSoundFile( 
				string cueSheet, 
				string acbPath, 
				string awbPath )
			{
				CueSheetName = cueSheet;
				AcbPath      = acbPath;
				AwbPath      = awbPath;
			}
		}

		//==============================
		//	変数
		//==============================
		private string								m_rootDir = string.Empty;
		private Dictionary<string,LocalSoundFile>   m_table   = new Dictionary<string, LocalSoundFile>();
		//==============================
		//	関数
		//==============================

		/// <summary>
		/// 
		/// </summary>
		public LocalSoundDatabase( string rootDir, IEnumerable<string> assetList )
		{
			m_rootDir	=rootDir;

			m_table		= assetList
				.Where	( c => c.StartsWith( rootDir ) )
				.Select ( c => c.Replace( rootDir, string.Empty ))
				.GroupBy( p => Path.GetFileNameWithoutExtension( p ) )
				.ToDictionary(
					g => g.Key,
					g => new LocalSoundFile
					(
						acbPath : g.FirstOrDefault( c => c.Contains( ".acb" ) ),
						awbPath : g.FirstOrDefault( c => c.Contains( ".awb" ) ) ?? string.Empty,
						cueSheet:g.Key
					) );
		}

		public bool Contains( string cueSheetName )
		{
			return m_table.ContainsKey( cueSheetName );
		}

		public void Dispose()
		{
			m_table.Clear();
		}

		public ISoundFileData Find( string cueSheetName )
		{
			LocalSoundFile value = null;
			m_table.TryGetValue(cueSheetName, out value);
			return value;
		}

		IEnumerator<ISoundFileData> IEnumerable<ISoundFileData>.GetEnumerator()
		{
			foreach( var d in m_table.Values )
			{
				yield return ( d as ISoundFileData );
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return m_table.Values.GetEnumerator();
		}
	}

	public static class ISoundFileDataExtensions
	{
		private static StringBuilder ms_builder = new StringBuilder();
		public static string ToDetail( this ISoundFileData self )
		{
			ms_builder.Length = 0;
			ms_builder
				.AppendLine( "[Path]" )
				.AppendLine( self.CueSheetName )
				.AppendLine( "[Acb]" )
				.AppendFormat( "Hash : {0}", self.AcbHash ).AppendLine()
				.AppendLine()
				.AppendFormat( "Size : {0}MB({1})", self.AcbSize/1024/1024, self.AcbSize ).AppendLine()
				.AppendLine()
				.AppendLine( "[Awb]" )
				.AppendFormat( "Hash : {0}", self.AwbHash ).AppendLine()
				.AppendLine()
				.AppendFormat( "Size : {0}MB({1})", self.AwbSize/1024/1024, self.AwbSize ).AppendLine()
				.AppendLine()
				;

			return ms_builder.ToString();
		}
	}
}
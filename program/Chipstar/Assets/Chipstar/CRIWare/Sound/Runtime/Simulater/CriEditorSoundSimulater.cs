#if UNITY_EDITOR

using System;
using System.Collections;
using System.Collections.Generic;
using Chipstar.Downloads;
using UnityEngine;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;

namespace Chipstar.Downloads.CriWare
{
	/// <summary>
	/// エディタでの挙動確認用
	/// ファイルを直接参照する
	/// </summary>
	public sealed class CriEditorSoundSimulater : ICriSoundFileManager
	{
		//	サウンドダミー情報
		private sealed class SoundFileSetData : ISoundFileData
		{
			public string CueSheetName { get; set; }
			public string AcbPath { get; set; }
			public string AwbPath { get; set; }

			public string AwbHash { get { return string.Empty; } }
			public string AcbHash { get { return string.Empty; } }

			public long AwbSize { get { return HasAwb() && File.Exists( AwbPath ) ? new FileInfo( AwbPath ).Length : 0; } }
			public long AcbSize { get { return File.Exists( AcbPath ) ? new FileInfo( AcbPath ).Length : 0; } }
			public bool IsIncludeFlag { private get; set; }
			public string AssetVersion { get { return string.Empty; } }

			public bool HasAwb()
			{
				return !string.IsNullOrEmpty(AwbPath );
			}

			public bool IsInclude() { return IsIncludeFlag; }
		}
		//=============================
		//	変数
		//=============================
		private ContentGroupConfig              m_soundConfig       = null;

		private IAccessPoint m_sourceDirPath = null;    //	保存先

		private IAccessPoint m_soundIncludeDir = null;  //	内包データパス
		private IAccessPoint m_soundRawDir = null; //	ダウンロード先

		private Dictionary<string, SoundFileSetData> m_remoteSoundTable	= new Dictionary<string, SoundFileSetData>();
		private Dictionary<string, SoundFileSetData> m_localSoundTable	= new Dictionary<string, SoundFileSetData>();

		private bool                            m_isLogin           = false;
		//	プロパティ
		//=============================

		//=============================
		//	関数
		//=============================

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public CriEditorSoundSimulater(
			IAccessPoint dlSourceDir,
			ContentGroupConfig	soundConfig
		)
		{
			m_sourceDirPath	 = dlSourceDir;
			m_soundIncludeDir= new AccessPoint( Application.streamingAssetsPath ).ToAppend( soundConfig.RemoteDirPath );

			m_soundConfig	= soundConfig;
		}

		/// <summary>
		/// 破棄
		/// </summary>
		public void Dispose()
		{
			m_remoteSoundTable.Clear();
		}

		/// <summary>
		/// 初期化
		/// </summary>
		public IEnumerator Setup( string includeDir, string localFileName )
		{
			m_localSoundTable   = ToSoundFileDict( m_soundIncludeDir.BasePath, true );
			yield return null;
		}
		/// <summary>
		/// SVデータ取得
		/// </summary>
		public IEnumerator Login( IAccessPoint dlServerRoot )
		{
			m_soundRawDir		= m_sourceDirPath.ToAppend( m_soundConfig.RemoteDirPath );
			m_remoteSoundTable	= ToSoundFileDict( m_soundRawDir.BasePath, false );

			m_isLogin			= true;
			yield break;
		}

		public void Logout()
		{
			m_remoteSoundTable.Clear();
			m_isLogin = false;
		}

		private Dictionary<string,SoundFileSetData> ToSoundFileDict( string dirPath, bool isInclude )
		{
			if( !Directory.Exists( dirPath ) )
			{
				//CriUtils.Warning( "[CRI Manager] Directory is Not Exists {0}", dirPath );
				return new Dictionary<string, SoundFileSetData>();
			}
			var regex = new Regex( "(.*?).(acb|awb)(?!.meta)");
			//	acbとawbの一覧
			var files = Directory
							.GetFiles( dirPath, "*", SearchOption.AllDirectories )
							.Select  ( p => p.ToConvertDelimiter( ))
							.Where   ( p => regex.IsMatch( p ))
							.ToArray ();
			//	ファイル名をキーとしてacbとawbをグルーピング
			return files
				.GroupBy( p => Path.GetFileNameWithoutExtension( p ) )
				.ToDictionary(
					g => g.Key,
					g => new SoundFileSetData
					{
						AcbPath         = g.FirstOrDefault( c => c.Contains( ".acb" ) ).Replace( dirPath, string.Empty ),
						AwbPath         = g.FirstOrDefault( c => c.Contains( ".awb" ) ) ?? string.Empty,
						CueSheetName    = g.Key,
						IsIncludeFlag	= isInclude
					} );
		}
		#region Sound
		/// <summary>
		/// 内包サウンドの置き場
		/// </summary>
		public IAccessPoint GetFileDir( string cueSheetName )
		{
			if( !m_isLogin )
			{
				return m_soundIncludeDir;
			}
			if( m_remoteSoundTable.ContainsKey( cueSheetName ))
			{
				return m_soundRawDir;
			}
			return m_soundIncludeDir;
		}
		/// <summary>
		/// DL予定ファイルの検索
		/// </summary>
		public ISoundFileData FindDLData( string cueSheetName )
		{
			if(!m_remoteSoundTable.ContainsKey(cueSheetName))
			{
				return null;
			}
			return m_remoteSoundTable[cueSheetName];
		}
		/// <summary>
		/// 内包サウンドの検索
		/// </summary>
		public ISoundFileData FindLocalData( string cueSheetName )
		{
			if(!m_localSoundTable.ContainsKey(cueSheetName))
			{
				return null;
			}
			return m_localSoundTable[cueSheetName];
		}
		/// <summary>
		/// 
		/// </summary>
		public bool HasFile( string cueSheetName )
		{
			if( m_remoteSoundTable.ContainsKey( cueSheetName ) )
			{
				return true;
			}
			//CriUtils.Log( "Not Found Remote DB: {0}",cueSheetName );

			if( m_localSoundTable.ContainsKey( cueSheetName ) )
			{
				return true;
			}
			//CriUtils.Log( "Not Found Local DB: {0}",cueSheetName );

			return false;
		}

		public IEnumerator Prepare( string cueSheetName )
		{
			yield return null;
		}

		public IEnumerable<ISoundFileData> GetDLDataList()
		{
			foreach( var f in m_remoteSoundTable.Values)
			{
				yield return f;
			}
		}
		#endregion

		public IEnumerator StorageClear()
		{
			yield return null;
		}
		public void DoUpdate() { }

		public IEnumerable<ISoundFileData> GetNeedDLList()
		{
			return new ISoundFileData[ 0 ];
		}
	}
}
#endif
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
	public sealed class CriEditorMovieSimulater : ICriMovieFileManager
	{
		//	動画ダミー情報
		private sealed class MovieFileSetData : IMovieFileData
		{
			public string   Key	 { get; set; }
			public string	Hash { get { return string.Empty; } }
			public string	Path { get; set; }
			public long		Size { get { return 0; } }
			public bool		IsIncludeFlag { private get; set; }
			public string AssetVersion { get { return string.Empty; } }

			public bool IsInclude()
			{
				return IsIncludeFlag;
			}
		}

		//=============================
		//	変数
		//=============================
		private ContentGroupConfig m_movieConfig = null;
		private OperationRoutine m_routine = new OperationRoutine();
		private IAccessPoint m_sourceDirPath;
		private IAccessPoint m_movieIncludeDir   = null; //	内包データパス
		private IAccessPoint m_movieRawDir       = null; //	ダウンロード先

		private Dictionary<string, MovieFileSetData> m_remoteMovieTable = new Dictionary<string, MovieFileSetData>();
		private Dictionary<string, MovieFileSetData> m_localMovieTable  = new Dictionary<string, MovieFileSetData>();

		private bool m_isLogin           = false;
		//=============================
		//	プロパティ
		//=============================

		//=============================
		//	関数
		//=============================

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public CriEditorMovieSimulater( 
			IAccessPoint			dlSourceDir, 
			ContentGroupConfig	movieConfig
		)
		{
			m_sourceDirPath	 = dlSourceDir;
			m_movieIncludeDir= m_sourceDirPath.ToAppend( movieConfig.RemoteDirPath );
			m_movieConfig   = movieConfig;
		}

		/// <summary>
		/// 破棄
		/// </summary>
		public void Dispose()
		{
			m_routine.Clear();
			m_remoteMovieTable.Clear();
		}

		/// <summary>
		/// 初期化
		/// </summary>
		public IEnumerator Setup( string includePath, string fileName )
		{
			m_localMovieTable	= ToMovieFileDict( m_movieIncludeDir.BasePath, true );
			yield return null;
		}
		/// <summary>
		/// SVデータ取得
		/// </summary>
		public IEnumerator Login( IAccessPoint dlServerRoot )
		{
			m_movieRawDir       = m_sourceDirPath.ToAppend( m_movieConfig.RemoteDirPath );
			m_remoteMovieTable  = ToMovieFileDict( m_movieRawDir.BasePath, false );
			m_isLogin			= true;
			yield break;
		}

		public void Logout()
		{
			m_remoteMovieTable.Clear();
			m_isLogin = false;
		}

		private Dictionary<string, MovieFileSetData> ToMovieFileDict( string dirPath, bool isInclude )
		{
			if( !Directory.Exists( dirPath ) )
			{
				//CriUtils.Warning( "[CRI Manager] Directory is Not Exists {0}", dirPath );
				return new Dictionary<string, MovieFileSetData>();
			}
			var regex = new Regex( "(.*?).(usm)$");
			//	acbとawbの一覧
			var files = Directory
							.GetFiles( dirPath, "*", SearchOption.AllDirectories )
							.Select  ( p => p.ToConvertDelimiter( ))
							.Where   ( p => regex.IsMatch( p ))
							.ToArray ();
			//	拡張子なしの相対パスをキーとする
			return files
				.ToDictionary(
					c => c.Replace( Path.GetExtension( c ), string.Empty ).Replace( dirPath, string.Empty ),
					c => new MovieFileSetData
					{
						Key             = c.Replace( Path.GetExtension( c ), string.Empty ),
						Path			= c,
						IsIncludeFlag   = isInclude
					} );
		}


		#region Movie

		public IAccessPoint GetFileDir(string key)
		{
			if (!m_isLogin)
			{
				return m_movieIncludeDir;
			}
			if (m_remoteMovieTable.ContainsKey(key))
			{
				return m_movieRawDir;
			}
			return m_movieIncludeDir;
		}
		public IPreloadOperation Prepare( string path )
		{
			return m_routine.Register(new PreloadOperation(SkipLoadProcess.Default));
		}

		public IMovieFileData FindDLData( string path )
		{
			if (!m_remoteMovieTable.ContainsKey(path))
			{
				return null;
			}
			return m_remoteMovieTable[ path ];
		}

		public IMovieFileData FindLocalData( string path )
		{
			if( !m_localMovieTable.ContainsKey(path))
			{
				return null;
			}
			return m_localMovieTable[ path ];
		}
		public bool HasFile( string path )
		{
			if( m_remoteMovieTable.ContainsKey( path ) )
			{
				return true;
			}
			//CriUtils.Log( "Not Found Remote DB: {0}", path );

			if( m_localMovieTable.ContainsKey( path ) )
			{
				return true;
			}
			//CriUtils.Log( "Not Found Local DB: {0}", path );

			return false;
		}
		public IEnumerable<IMovieFileData> GetDLDataList()
		{
			foreach( var f in m_remoteMovieTable.Values )
			{
				yield return f;
			}
		}

		#endregion

		public IEnumerator StorageClear()
		{
			yield return null;
		}
		public void DoUpdate()
		{
			m_routine?.Update();
		}

		public IEnumerable<IMovieFileData> GetNeedDLList()
		{
			return new IMovieFileData[ 0 ];
		}

		public void Stop()
		{
			m_routine.Clear();
		}
	}
}
#endif
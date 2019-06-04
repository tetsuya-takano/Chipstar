using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Chipstar.Downloads.CriWare
{
	public interface IMovieFileDatabase : IDisposable, IEnumerable<IMovieFileData>
	{
		bool			Contains( string path );
		IMovieFileData	Find	( string path );
	}

	public interface IMovieFileData
	{
		string Key  { get; }
		string Path { get; }
		string Hash { get; }
		long   Size { get; }
		string AssetVersion { get; }

		bool IsInclude();
	}

	/// <summary>
	/// サウンドファイルのテーブル
	/// サウンドのバージョンチェック用
	/// </summary>
	[Serializable]
	public sealed class MovieFileDatabase : IMovieFileDatabase
	{
		//=============================
		//	const
		//=============================
		private const string USM_FORMAT = "{0}.usm";
		private static readonly Encoding Encode = new UTF8Encoding( false );
		//=============================
		//	class
		//=============================
		[Serializable]
		private class MovieFileData : IMovieFileData
		{
			public string   Path;
			public string	Key;        //	階層
			public long     Size;
			public string	Hash;
			public string   AssetVersion;

			public MovieFileData( IMovieFileData data )
			{
				Key  = data.Key;
				Path = data.Path;
				Size = data.Size;
				Hash = data.Hash;
				AssetVersion = data.AssetVersion;
			}
			string IMovieFileData.Key  { get { return Key; } }
			string IMovieFileData.Path { get { return Path; } }
			string IMovieFileData.Hash { get { return Hash; } }
			long IMovieFileData.Size { get { return Size; } }
			string IMovieFileData.AssetVersion { get { return AssetVersion; } }

			bool IMovieFileData.IsInclude()
			{
				//	DLファイルなのでfalse固定
				return false;
			}
		}
		[SerializeField] private List<MovieFileData> m_list = new List<MovieFileData>();

		//=============================
		//	関数
		//=============================
		public static MovieFileDatabase Read( string path )
		{
			if( !File.Exists( path ) )
			{
				return new MovieFileDatabase();
			}
			var json = File.ReadAllText( path, Encode );
			return JsonUtility.FromJson<MovieFileDatabase>( json );
		}
		public static bool Write( string path, MovieFileDatabase table )
		{
			var json = JsonUtility.ToJson( table, true );
			if( string.IsNullOrWhiteSpace( json ) )
			{
				return false;
			}
			File.WriteAllText( path, json, Encode );
			return true;
		}
		/// <summary>
		/// 破棄
		/// </summary>
		public void Dispose()
		{

		}

		public void Add( IMovieFileData data )
		{
			m_list.Add( new MovieFileData( data ) );
		}

		/// <summary>
		/// 所持判定
		/// </summary>
		public bool Contains( string key )
		{
			return m_list.Any( c => c.Key == key );
		}

		IEnumerator<IMovieFileData> IEnumerable<IMovieFileData>.GetEnumerator()
		{
			foreach( var d in m_list)
			{
				yield return (d as IMovieFileData);
			}
		}
		IEnumerator IEnumerable.GetEnumerator()
		{
			return m_list.GetEnumerator();
		}

		public IMovieFileData Find( string path )
		{
			return m_list.FirstOrDefault( c => c.Key == path );
		}
	}

	public sealed class LocalMovieDatabase : IMovieFileDatabase
	{
		/// <summary>
		/// 内包MOVIE情報クラス
		/// </summary>
		private sealed class LocalMovieFile : IMovieFileData
		{
			public string Key  { get; private set; }
			public string Path { get; private set; }

			public bool IsInclude()
			{
				//	内包データなのでtrue固定
				return true;
			}
			//======== 内包データなので考えなくてよい
			public string	Hash { get { return string.Empty; } }
			public long		Size { get { return 0; } }
			public string	AssetVersion { get { return string.Empty; } }

			public LocalMovieFile( string path )
			{
				Path	= path;
				Key     = Path.Replace( System.IO.Path.GetExtension( path),string.Empty );
			}
		}

		//==============================
		//	変数
		//==============================
		private string					m_rootDir = string.Empty;
		private List<LocalMovieFile>    m_list    = new List<LocalMovieFile>();
		//==============================
		//	関数
		//==============================

		/// <summary>
		/// 
		/// </summary>
		public LocalMovieDatabase( string rootDir, IEnumerable<string> assetList )
		{
			m_rootDir	=rootDir;

			m_list		= assetList
				.Where	( c => c.StartsWith( rootDir ) )
				.Select ( c => c.Replace( rootDir, string.Empty ))
				.Select ( c => new LocalMovieFile( path: c ) )
				.ToList();
		}

		public void Dispose()
		{
			m_list.Clear();
		}

		public bool Contains( string key )
		{
			return m_list.Any( c => c.Key == key );
		}

		IEnumerator<IMovieFileData> IEnumerable<IMovieFileData>.GetEnumerator()
		{
			foreach( var d in m_list )
			{
				yield return ( d as IMovieFileData );
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return m_list.GetEnumerator();
		}

		public IMovieFileData Find( string key )
		{
			return m_list.FirstOrDefault( c => c.Key == key );
		}
	}
	public static class IMovieFileDataExtensions
	{
		private static StringBuilder ms_builder = new StringBuilder();
		public static string ToDetail( this IMovieFileData self )
		{
			ms_builder.Length = 0;
			ms_builder
				.AppendLine( "[Path]" )
				.AppendLine( self.Path )
				.AppendFormat( "Hash : {0}", self.Hash ).AppendLine()
				.AppendLine()
				.AppendFormat( "Size : {0}MB({1})", self.Size/1024/1024, self.Size ).AppendLine()
				.AppendLine()
				;

			return ms_builder.ToString();
		}
	}
}
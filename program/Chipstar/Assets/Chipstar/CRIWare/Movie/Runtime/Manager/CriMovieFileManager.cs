﻿using Chipstar.Downloads;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Chipstar.Downloads.CriWare
{
	public interface ICriMovieFileManager : ICriFileManager
	{
		IAccessPoint GetFileDir( string key );

		IPreloadOperation Prepare(string key);

		IMovieFileData FindDLData(string key);
		IMovieFileData FindLocalData(string key);

		bool HasFile(string key);

		IEnumerable<IMovieFileData> GetDLDataList();
		IEnumerable<IMovieFileData> GetNeedDLList();
	}
	/// <summary>
	/// Criのファイル管理をする
	/// </summary>
	public class CriMovieFileManager : CriFileManager, ICriMovieFileManager
	{
		
		//=====================================
		//	変数
		//=====================================
		private LocalFileExporter   m_exporter          = null;   //	内包リソース出力機(OBB用)
		private Encoding m_encoding = null;

		//--------------- ローカルデータ情報
		private StreamingAssetsDatabase m_streamingAssetsDB = null;
		private LocalMovieDatabase      m_localDB      = null; //	内包ムービーのテーブル
		private CriVersionTableJson     m_cacheDB      = null; //	ムービーの保持バージョンファイル
		private IMovieFileDatabase      m_remoteDB     = null;  //	データベース情報

		//=====================================
		//	プロパティ
		//=====================================

		//=====================================
		//	関数
		//=====================================
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public CriMovieFileManager(
			IAccessPoint includeStorage,
			IAccessPoint downloadStorage,
			StreamingAssetsDatabase streamingAssetsDatabase,
			IJobEngine engine
		) : base(downloadStorage, includeStorage, engine)
		{
			//	-------------------
			//	共通
			//	-------------------
			m_encoding          = new UTF8Encoding( false );
			m_streamingAssetsDB = streamingAssetsDatabase;
			m_exporter          = LocalFileExporter.Create( IncludeStorage );

			//	-------------------
			//	ムービー
			//	-------------------
		}

		protected override void DoInit(ICriDownloader downloader)
		{
			downloader.OnInstalled = (key, hash) =>
			{
				m_cacheDB.Replace(key, hash);
				DoDatabaseSave();
			};
		}

		/// <summary>
		/// 
		/// </summary>
		protected override void DoDispose()
		{
			m_remoteDB     = null;
		}

		/// <summary>
		/// ローカルDB取得
		/// </summary>
		protected override IEnumerator DoSetup( string includeDirPath )
		{
			yield return m_streamingAssetsDB.InitLoad();
			m_localDB = new LocalMovieDatabase( includeDirPath, m_streamingAssetsDB.AssetList );
			m_cacheDB = CriVersionTableJson.ReadLocal( CacheDbLocation.FullPath, m_encoding );
			ChipstarLog.Log_ReadLocalTable(m_cacheDB, CacheDbLocation);
			//	StreamingAssets内のファイルをOBB用に出力
			foreach ( var movie in m_localDB )
			{
				var usm = Path.Combine( includeDirPath, movie.Path );
				yield return m_exporter.Run( usm );
			}
			yield break;
		}

		/// <summary>
		/// リモートDB取得
		/// </summary>
		protected override IEnumerator DoLogin( string json )
		{
			m_remoteDB = JsonUtility.FromJson<MovieFileDatabase>(json);
			ChipstarLog.AssertNotNull(m_remoteDB, $"Movie Remote DB is Null : {RemoteDbLocation.FullPath }");
			if (m_remoteDB == null)
			{
				m_remoteDB = new MovieFileDatabase();
			}
			yield break;
		}

		/// <summary>
		/// リモートのデータを破棄する
		/// </summary>
		protected override void DoLogout()
		{
			m_remoteDB.DisposeIfNotNull();
			m_remoteDB = null;
		}
		#region Movie

		/// <summary>
		/// ムービー準備
		/// </summary>
		public IPreloadOperation Prepare( string key)
		{
			var process = PrepareImpl(key);
			return AddQueue(new PreloadOperation(process));
		}
		private ILoadProcess PrepareImpl( string key )
		{
			if( m_remoteDB == null )
			{
				if( IsExistsLocalMovieFile( key ))
				{
					return SkipLoadProcess.Default;
				}
				//	TODO : 保存済みファイルを調べる
				return SkipLoadProcess.Default;
			}
			//	あったら落とす
			var fileData = m_remoteDB.Find( key );
			if( fileData == null)
			{
				//CriUtils.Warning( "Movie File Key Not Found : {0}", key );
				return SkipLoadProcess.Default;
			}
			if (HasUsm(fileData))
			{
				return SkipLoadProcess.Default;
			}
			//	Usmファイルを落とす

			return Download(relativePath: fileData.Path, fileVersion: fileData.Hash, size: fileData.Size);
		}

		/// <summary>
		/// DL予定動画情報の取得
		/// </summary>
		public IMovieFileData FindDLData( string key )
		{
			if( m_remoteDB == null )
			{
				return null;
			}
			return m_remoteDB.Find( key );
		}
		/// <summary>
		/// 内包動画情報の取得
		/// </summary>
		public IMovieFileData FindLocalData( string path )
		{
			return m_localDB.Find( path );
		}

		/// <summary>
		/// 動画存在チェック
		/// </summary>
		public bool HasFile( string path )
		{
			if( IsExistsRemoteMovieFile( path ))
			{
				return true;
			}
			return IsExistsLocalMovieFile( path );
		}

		/// <summary>
		/// 動画DL済みかどうか
		/// </summary>
		private bool IsExistsRemoteMovieFile( string key )
		{
			if( m_remoteDB == null )
			{
				return false;
			}

			if( !m_remoteDB.Contains( key ))
			{
				return false;
			}

			var data = m_remoteDB.Find( key );
			if (!HasCacheFile(data.Path, data.Hash, data.Size))
			{
				return false;
			}

			return true;
		}
		private bool HasUsm( IMovieFileData data )
		{
			if( data == null )
			{
				return false;
			}
			return HasCacheFile(data.Path, data.Hash, data.Size);
		}

		private bool HasCacheFile(string path, string hash, long size)
		{
			//	バージョン不一致
			if (!m_cacheDB.IsSameVersion(path, hash))
			{
				return false;
			}
			if (IsBreakFile(path, size))
			{
				return false;
			}
			return true;
		}

		/// <summary>
		/// 内包ファイルかどうか
		/// </summary>
		private bool IsExistsLocalMovieFile( string path )
		{
			var isExists = m_localDB.Contains( path );
			if( !isExists )
			{
				//CriUtils.LogDetail( "Not Exists Local DB:: {0}", path );
			}
			return isExists;
		}

		/// <summary>
		/// 動画ファイル配置先
		/// </summary>
		public IAccessPoint GetFileDir( string key )
		{
			if( IsExistsRemoteMovieFile( key ) )
			{
				return CacheStorage;
			}
			return IncludeStorage;
		}

		/// <summary>
		/// 
		/// </summary>
		public IEnumerable<IMovieFileData> GetDLDataList()
		{
			if( m_remoteDB == null )
			{
				return new IMovieFileData[0];
			}

			return m_remoteDB;
		}


		/// <summary>
		/// DLしてない動画リストを取得
		/// </summary>
		public IEnumerable<IMovieFileData> GetNeedDLList()
		{
			if( m_remoteDB == null )
			{
				return new IMovieFileData[ 0 ];
			}

			return m_remoteDB
						.Where( c =>
						{
							return m_cacheDB.IsSameVersion( c.Key, c.Hash );
						} )
						.ToArray(); ;
		}
		#endregion

		/// <summary>
		/// ムービーのセーブデータを保存
		/// </summary>
		protected override void DoDatabaseSave()
		{
			m_cacheDB.WriteFile( CacheDbLocation.FullPath, m_encoding );
		}
		protected override void DoDatabaseClear()
		{
			m_cacheDB = new CriVersionTableJson();
		}

	}
}
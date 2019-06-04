using Chipstar.Downloads;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Chipstar.Downloads.CriWare
{
	public interface ICriSoundFileManager : IDisposable
	{
		IAccessPoint GetFileDir( string cueSheetName );

		IEnumerator		Setup( string includeRelativePath, string saveFileName );
		IEnumerator		Login( IAccessPoint accessPoint );
		void			Logout();


		IEnumerator		Prepare( string cueSheetName );

		ISoundFileData	FindDLData		( string cueSheetName );
		ISoundFileData	FindLocalData	( string cueSheetName );

		bool HasFile( string cueSheetName );

		IEnumerable<ISoundFileData> GetDLDataList();

		IEnumerable<ISoundFileData> GetNeedDLList();

		IEnumerator StorageClear();
		void DoUpdate();
	}
	/// <summary>
	/// Criのファイル管理をする
	/// </summary>
	public class CriSoundFileManager : CriFileManager, ICriSoundFileManager
	{
		
		//=====================================
		//	変数
		//=====================================
		private LocalFileExporter m_exporter = null;   //	内包リソース出力機(OBB用)
		private Encoding          m_encoding = null;

		//--------------- ローカルデータ情報
		private StreamingAssetsDatabase m_streamingAssetsDB = null;
		private LocalSoundDatabase  m_localDB       = null; // 内包サウンドのテーブル
		private CriVersionTableJson m_cacheDB       = null; // サウンドの保持バージョンファイル

		//--------------- リモートデータ情報
		private ISoundFileDatabase  m_remoteDB     = null; //	データベース情報

		//=====================================
		//	プロパティ
		//=====================================

		//=====================================
		//	関数
		//=====================================
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public CriSoundFileManager(
			IAccessPoint includeStorage,
			IAccessPoint downloadStorage,
			StreamingAssetsDatabase streamingAssetsDatabase
		) : base(downloadStorage, includeStorage )
		{
			m_encoding          = new UTF8Encoding( false );
			m_streamingAssetsDB = streamingAssetsDatabase;
			m_exporter          = LocalFileExporter.Create( IncludeStorage );
		}

		/// <summary>
		/// 
		/// </summary>
		protected override void DoDispose()
		{
			m_remoteDB		= null;
		}

		protected override void DoInit(ICriDownloader downloader)
		{
			downloader.OnInstalled    = (key, hash) => m_cacheDB.Replace(key, hash);
		}

		/// <summary>
		/// ローカルDB取得
		/// </summary>
		protected override IEnumerator DoSetup( string includeRelativePath )
		{
			yield return m_streamingAssetsDB.InitLoad();
			//	StreamingAssetsから内包サウンドの情報を引っ張り出す
			m_localDB = new LocalSoundDatabase( includeRelativePath, m_streamingAssetsDB.AssetList );
			m_cacheDB = CriVersionTableJson.ReadLocal( CacheDbLocation.FullPath, m_encoding );
			Chipstar.Log_ReadLocalTable(m_cacheDB, CacheDbLocation);

			//	StreamingAssets内のファイルをOBB用に出力

			foreach ( var sound in m_localDB )
			{
				var acb = Path.Combine( includeRelativePath, sound.AcbPath );
				var awb = Path.Combine( includeRelativePath, sound.AwbPath );
				yield return m_exporter.Run( acb );
				yield return m_exporter.Run( awb );
			}
		}

		/// <summary>
		/// リモートDB取得
		/// </summary>
		protected override IEnumerator DoLogin( string json )
		{
			m_remoteDB = JsonUtility.FromJson<SoundFileDatabase>( json );
			Chipstar.AssertNotNull(m_remoteDB, $"Sound Remote DB is Null : { RemoteDbLocation.FullPath }");
			if (m_remoteDB == null)
			{
				m_remoteDB = new SoundFileDatabase();
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
		#region Sound

		/// <summary>
		/// サウンド準備
		/// </summary>
		public IEnumerator Prepare( string cueSheetName )
		{
			//	そもそもあるかどうか
			if( IsExistsRemoteSoundFile( cueSheetName ))
			{
				//	DL済みファイルなので素通り
				yield break;
			}
			if( IsExistsLocalSoundFile( cueSheetName ))
			{
				//	ローカルファイルなので飛ばして良い
				yield break;
			}

			if( m_remoteDB == null)
			{
				yield break;
			}
			//	あったら落とす
			var fileData = m_remoteDB.Find( cueSheetName );
			if( fileData == null)
			{
				Chipstar.Log_RequestCueSheet_Error( cueSheetName );
				yield break;
			}
			Chipstar.Log_Download_Sound(fileData);
			//	Acbファイルを落とす
			{
				var job = Download(
							relativePath: fileData.AcbPath,
							fileVersion: fileData.AcbHash,
							size: fileData.AcbSize
						);
				yield return job;
			}

			//	Awbファイルがある場合
			if( fileData.HasAwb())
			{
				{
					var job = Download(
								relativePath: fileData.AwbPath,
								fileVersion: fileData.AwbHash,
								size: fileData.AwbSize
							);
					yield return job;
				}
			}
			DoDatabaseSave();
			yield break;
		}

		/// <summary>
		/// DL予定サウンド情報の取得
		/// </summary>
		public ISoundFileData FindDLData( string cueSheetName )
		{
			if( m_remoteDB == null )
			{
				return null;
			}
			return m_remoteDB.Find( cueSheetName );
		}
		/// <summary>
		/// 内包サウンド情報の取得
		/// </summary>
		public ISoundFileData FindLocalData( string cueSheetName )
		{
			return m_localDB.Find( cueSheetName );
		}

		/// <summary>
		/// 存在判定
		/// </summary>
		public bool HasFile( string cueSheetName )
		{
			if( IsExistsRemoteSoundFile( cueSheetName ) )
			{
				//	リモートファイルの方にあるなら問題ナシ
				return true;
			}
			return IsExistsLocalSoundFile( cueSheetName );
		}

		private bool IsExistsRemoteSoundFile( string cueSheetName )
		{
			if( m_remoteDB == null )
			{
				return false;
			}
			if( !m_remoteDB.Contains( cueSheetName ) )
			{
				Chipstar.Log_NotContains_RemoteDB_Sound( cueSheetName );
				return false;
			}

			// サウンドデータ
			var data = m_remoteDB.Find( cueSheetName );

			// --- acb-check

			if (IsBreakFile(data.AcbPath, data.AcbSize))
			{
				return false;
			}
			if (!m_cacheDB.IsSameVersion(data.AcbPath, data.AcbHash))
			{
				// バージョン不一致 :: データ更新
				Chipstar.Log_MissMatchVersion(data.AcbPath, m_cacheDB.GetVersion(data.AcbPath), data.AcbHash);
				return false;
			}
			
			if (!data.HasAwb())
			{
				//	Awb無いならここまででいい
				return true;
			}

			// --- awb-check
			if( IsBreakFile(data.AwbPath,data.AwbSize))
			{
				return false;
			}

			//
			if (!m_cacheDB.IsSameVersion(data.AwbPath, data.AwbHash))
			{
				// バージョン不一致 :: 更新
				Chipstar.Log_MissMatchVersion(data.AwbPath, m_cacheDB.GetVersion(data.AwbPath), data.AwbHash);
				return false;
			}
			return true;
		}
		private bool IsExistsLocalSoundFile( string cueSheetName )
		{
			var isExists = m_localDB.Contains( cueSheetName );
			if( !isExists )
			{
				Chipstar.Log_NotContains_LocalDB_Sound( cueSheetName );
			}
			return isExists;
		}

		public IAccessPoint GetFileDir( string cueSheetName )
		{
			if( IsExistsRemoteSoundFile( cueSheetName ))
			{
				return CacheStorage;
			}
			return IncludeStorage;
		}
		/// <summary>
		/// 
		/// </summary>
		public IEnumerable<ISoundFileData> GetDLDataList()
		{
			if( m_remoteDB == null)
			{
				return new ISoundFileData[ 0 ];
			}

			return m_remoteDB;
		}

		/// <summary>
		/// DLしてないサウンドリストを取得
		/// </summary>
		public IEnumerable<ISoundFileData> GetNeedDLList()
		{
			if( m_remoteDB == null)
			{
				return new ISoundFileData[ 0 ];
			}

			return m_remoteDB
						.Where( c => 
						{
							//	ACB / AWB 両方が最新かどうか
							return	m_cacheDB.IsSameVersion( c.CueSheetName, c.AcbHash )
							||		m_cacheDB.IsSameVersion( c.CueSheetName, c.AwbHash );
						} )
						.ToArray(); ;
		}

		#endregion

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
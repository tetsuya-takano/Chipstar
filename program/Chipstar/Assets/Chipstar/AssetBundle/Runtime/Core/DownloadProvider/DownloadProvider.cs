using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chipstar.Downloads
{
    public interface IDownloadProvider : IDisposable
	{
		IEnumerator InitLoad(IAccessPoint accessPoint);
		ILoadProcess CacheOrDownload(string name);
		ILoadProcess LoadFile( string name );

		void DoUpdate();
		void Cancel();
	}

	/// <summary>
	/// 読み込みまわりの管理
	/// ダウンロード / キャッシュロード付近
	/// </summary>
	public class DownloadProvider<TRuntimeData> 
                                : IDownloadProvider
            where TRuntimeData  : IRuntimeBundleData<TRuntimeData>

    {
		//===============================
		//  変数
		//===============================
		private List<ChipstarResultCode> m_errorList = new List<ChipstarResultCode>();
		private bool m_isRunning = false;

		//===============================
		//  プロパティ
		//===============================
		private IStorageDatabase StorageDatabase { get; set; } // ローカルストレージのキャッシュ情報
		private ILoadDatabase<TRuntimeData> LoadDatabase { get; set; } // コンテンツテーブルから作成したDB
		private IJobEngine JobEngine { get; set; } // DLエンジン
		private IJobCreator JobCreator { get; set; } // ジョブの作成
		private IAccessPoint Server { get; set; } // 接続先
		public Func<IAccessPoint, IAccessLocation> GetBuildMapLocation { private get; set; }
		public Func<IAccessPoint, TRuntimeData, IAccessLocation> GetBundleLocation { private get; set; }
		public Action<IReadOnlyList<ChipstarResultCode>> OnLoadError { set; private get; }
		//===============================
		//  関数
		//===============================

		public DownloadProvider
			(
				ILoadDatabase<TRuntimeData> loadDatabase,
				IStorageDatabase storageDatabase,
				IJobEngine dlEngine,
				IJobCreator jobCreator
			)
		{
			LoadDatabase = loadDatabase;
			StorageDatabase = storageDatabase;
			JobEngine = dlEngine;
			JobCreator = jobCreator;
		}

		public void Dispose()
		{
			JobCreator.Dispose();
			JobEngine.Dispose();

			StorageDatabase = null;
			LoadDatabase = null;
			JobCreator = null;
			JobEngine = null;

			GetBundleLocation = null;
			GetBuildMapLocation = null;

			m_errorList.Clear();
		}

		/// <summary>
		/// 初期化処理
		/// </summary>
		public IEnumerator InitLoad( IAccessPoint accessServer )
        {
			m_isRunning = true;
			Server = accessServer;
			Chipstar.Log_Downloader_StartInit( );
			//	初期化
			yield return LoadDatabase.Clear();

			//	アクセス先を保持

			//	コンテンツデータの取得
			var location = GetBuildMapLocation?.Invoke( Server );

			var loadBuildMap = DoInitielizeLoad( location );
			Chipstar.Log_Downloader_RequestBuildMap( location );
			while( !loadBuildMap.IsCompleted )
            {
                yield return null;
            }
			yield return LoadDatabase.Create( loadBuildMap.Content );
            yield break;
        }

		/// <summary>
		/// 更新処理
		/// </summary>
		public void DoUpdate()
		{
			if ( m_errorList.Count > 0  && m_isRunning )
			{
				//	統合エラー処理
				OnLoadError?.Invoke( m_errorList );
				m_isRunning = false;
				return;
			}
			JobEngine?.Update();
		}

		/// <summary>
		/// 初期化ロード処理
		/// </summary>
		protected virtual ILoadJob<byte[]> DoInitielizeLoad( IAccessLocation location )
		{
			return JobCreator.BytesLoad( JobEngine, location );
		}

		/// <summary>
		/// ダウンロード処理
		/// </summary>
		public ILoadProcess CacheOrDownload( string bundleName )
		{
			var data = LoadDatabase.GetBundleData( bundleName );
			if( data == null)
			{
				return SkipLoadProcess.Default;
			}
			if( data.IsOnMemory )
			{
				//	ロード済みは無視
				return SkipLoadProcess.Default;
			}
			if( StorageDatabase.HasStorage( data ))
			{
				//	キャッシュ済は無視
				return SkipLoadProcess.Default;
			}

			return CreateDowloadJob( data );
		}

		/// <summary>
		/// ロード処理
		/// </summary>
		public ILoadProcess LoadFile( string name )
		{
			var data = LoadDatabase.GetBundleData( name );
			if( data == null )
			{
				return SkipLoadProcess.Default;
			}
			if( data.IsOnMemory )
			{
				//	ロードしてあるならしない
				Chipstar.Log_Skip_OnMemory( data.Name );
				return SkipLoadProcess.Default;
			}
			//	ローカルファイルを開く
			return CreateLocalFileOpenJob( data );
		}

		/// <summary>
		/// ダウンロード
		/// </summary>
		protected virtual ILoadProcess CreateDowloadJob( TRuntimeData data )
		{
            var location    = GetBundleLocation?.Invoke( Server, data );
			if( JobEngine.HasRequest( location ) )
			{
				//	リクエスト済みのモノは無視
				return SkipLoadProcess.Default;
			}
			var localPath = StorageDatabase.ToLocation( data );
            var job = JobCreator.FileDL( JobEngine, location, localPath );
			Chipstar.Log_RequestDownload( data );
			return new LoadProcess<Empty>(
				job,
				onCompleted: ( _ ) =>
				{
					//	バージョンを保存
					StorageDatabase.Save( data );
					StorageDatabase.Apply();
				},
				onError: code => OnError(code)
			);
		}

		/// <summary>
		/// ローカルファイルを開く
		/// </summary>
		protected virtual ILoadProcess CreateLocalFileOpenJob( TRuntimeData data )
		{
			var location = StorageDatabase.ToLocation(data);
			if ( JobEngine.HasRequest( location ) )
			{
				//	リクエスト済みのモノは完了まで待つ
				return new WaitLoadProcess( () => data.IsOnMemory );
			}

			var job	= JobCreator.OpenLocalBundle( JobEngine, location, data.Hash, data.Crc );
			return new LoadProcess<AssetBundle>(
				job,
				onCompleted: ( content ) =>
				{
					data.OnMemory( content );
				},
				onError : code => OnError( code )
			);
		}
		private void OnError( ChipstarResultCode code )
		{
			m_errorList.Add( code );
		}

		public void Cancel()
		{
			m_isRunning = false;
			JobEngine?.Cancel();
			m_errorList.Clear();
		}
	}
}
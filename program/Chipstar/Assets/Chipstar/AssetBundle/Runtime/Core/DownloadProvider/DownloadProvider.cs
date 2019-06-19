using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chipstar.Downloads
{
    public interface IDownloadProvider : IDisposable
	{
		Action<ResultCode> OnDownloadError { set; }

		ILoadProcess InitLoad(IAccessPoint accessPoint);
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
		private bool IsRunning = false;

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

		public Action<ResultCode> OnDownloadError { set; private get; }
		public Action OnStartAny { set; private get; }
		public Action OnStopAny { set; private get; }
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
			OnDownloadError = null;
			OnStartAny = null;
			OnStopAny = null;
		}

		/// <summary>
		/// 初期化処理
		/// </summary>
		public ILoadProcess InitLoad( IAccessPoint accessServer )
        {
			ChipstarLog.Log_Downloader_StartInit();
			// リセット
			IsRunning = true;
			Server = accessServer;
			LoadDatabase.Clear();
			//	コンテンツデータの取得
			var location = GetBuildMapLocation?.Invoke( Server );
			var loadBuildMap = DoInitielizeLoad( location );
			var process = new LoadProcess<byte[]>(
				job : AddJob( loadBuildMap ), 
				onCompleted : c => 
				{
					LoadDatabase.Create( c );
				}, 
				onError: (code) => OnError(code)
			);
			return process;
        }

		/// <summary>
		/// 更新処理
		/// </summary>
		public void DoUpdate()
		{
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
				ChipstarLog.Log_Skip_OnMemory( data.Name );
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
			var job = JobCreator.FileDL(JobEngine, location, localPath);
			return new LoadProcess<Empty>(
				AddJob( job ),
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
				AddJob( job ),
				onCompleted: ( content ) =>
				{
					data.OnMemory( content );
				},
				onError : code => OnError( code )
			);
		}

		private T AddJob<T>( T job ) where T : ILoadJob
		{
			job.OnStart = (_) => OnStartAny?.Invoke();
			job.OnStop = (_) => OnStopAny?.Invoke();

			return job;
		}

		private void OnError( ResultCode code )
		{
			OnDownloadError?.Invoke( code );
		}

		public void Cancel()
		{
			IsRunning = false;
			JobEngine?.Cancel();
		}
	}
}
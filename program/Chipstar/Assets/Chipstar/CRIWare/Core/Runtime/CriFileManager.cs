using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Chipstar.Downloads.CriWare
{
	/// <summary>
	/// Manfestファイル取得用のデリゲート
	/// </summary>
	public delegate IAccessLocation OnGetManifestDelegate( IAccessPoint server );
	/// <summary>
	/// ファイルアクセス用のデリゲート
	/// </summary>
	public delegate IAccessLocation OnGetFileDelegate( IAccessPoint server, string relativePath );

	/// <summary>
	/// Cri用のマネージャ
	/// </summary>
	public abstract class CriFileManager : IDisposable
	{
		//====================================
		//	プロパティ
		//====================================

		/// <summary>
		/// 接続先
		/// </summary>
		protected IAccessPoint AccessPoint { get; private set; }
		/// <summary>
		/// 保存ディレクトリ
		/// </summary>
		protected IAccessPoint CacheStorage { get; private set; }
		/// <summary>
		/// 内包ディレクトリ
		/// </summary>
		protected IAccessPoint IncludeStorage { get; private set; }

		protected IAccessLocation RemoteDbLocation { get; private set; }
		protected IAccessLocation CacheDbLocation { get; private set; }

		/// <summary>
		/// DLエンジン
		/// </summary>
		protected IJobEngine Engine { get; private set; }
		private ICriDownloader Downloader { get; set; }
		private OperationRoutine Routine { get; set; }
		private List<ResultCode> ErrorList { get; } = new List<ResultCode>();
		private bool IsRunning { get; set; }

		public OnGetManifestDelegate GetManifestLocation { private get; set; }
		public OnGetFileDelegate GetFileDLLocation { private get; set; }
		public Action<IReadOnlyList<ResultCode>> OnLoadError { private get; set; }

		//====================================
		//	関数
		//====================================

		public void Dispose()
		{
			Downloader.DisposeIfNotNull();
			Downloader = null;
			Engine.DisposeIfNotNull();
			Engine = null;
			GetManifestLocation = null;
			GetFileDLLocation = null;
			ErrorList.Clear();
			DoDispose();
		}
		protected virtual void DoDispose() { }

		public CriFileManager(
			IAccessPoint cacheStorage,
			IAccessPoint includeStorage,
			IJobEngine engine
		)
		{
			CacheStorage   = cacheStorage;
			IncludeStorage = includeStorage;

			Engine         = engine;
			Routine        = new OperationRoutine();
			Downloader     = new CriDownloader( CacheStorage );
			Downloader.GetFileDLLocation = (server, relativePath) => GetFileDLLocation?.Invoke( server, relativePath );
			Downloader.GetSuccessDL = (file, size) => !IsBreakFile(file, size);
			Downloader.OnError = code => OnError( code );
			DoInit( Downloader );
		}

		protected abstract void DoInit( ICriDownloader downloader );

		/// <summary>
		/// 使用準備
		/// </summary>
		public IEnumerator Setup(string includeRelativePath, string saveFileName)
		{
			CacheDbLocation = CacheStorage.ToLocation( saveFileName );
			yield return DoSetup( includeRelativePath );
		}
		protected abstract IEnumerator DoSetup( string includeRelativePath );


		public IEnumerator Login( IAccessPoint server )
		{
			IsRunning = true;
			AccessPoint = server;
			RemoteDbLocation = GetManifestLocation?.Invoke( AccessPoint );

			var job = WRDL.GetTextFile( RemoteDbLocation );
			Engine.Enqueue( job );
			var waitForManifestDL = new WaitUntil(() => job.IsCompleted );
			yield return waitForManifestDL;

			var json = job.Content;
			yield return DoLogin( json );
			yield return Downloader.Init( AccessPoint );
		}

		protected abstract IEnumerator DoLogin( string json );

		public void Logout()
		{
			DoLogout();
		}
		protected abstract void DoLogout();

		protected ILoadProcess Download( string relativePath, string fileVersion, long size )
		{
			return Downloader.Start( Engine, relativePath, fileVersion, size );
		}
		protected T AddQueue<T>(T operation) where T : ILoadOperater
		{
			operation.OnError = code => OnError(code);
			return Routine.Register(operation);
		}

		/// <summary>
		/// 保存済みファイルの情報を取得
		/// </summary>
		protected FileInfo GetCacheFileInfo( string relativePath )
		{
			var location = CacheStorage.ToLocation(relativePath);
			if( File.Exists(location.FullPath))
			{
				return new FileInfo( location.FullPath );
			}
			return null;
		}

		protected bool IsBreakFile( string relativePath, long size )
		{
			var info = GetCacheFileInfo(relativePath);
			if (info == null || !info.Exists)
			{
				//	持ってない :: 新規DL
				ChipstarLog.Log_NotFound_Downloaded_File(relativePath);
				return true;
			}
			if (info.Length != size)
			{
				// サイズ違い :: 破損
				ChipstarLog.Log_MaybeFileBreak(info, size);
				return true;
			}
			return false;
		}

		public IEnumerator StorageClear()
		{
			if( Directory.Exists( CacheStorage.BasePath ))
			{
				Directory.Delete(CacheStorage.BasePath, true);
			}
			DoDatabaseClear();
			DoDatabaseSave();
			yield break;
		}

		protected abstract void DoDatabaseClear();
		/// <summary>
		/// セーブデータ保存
		/// </summary>
		protected abstract void DoDatabaseSave();

		public void DoUpdate()
		{
			if( ErrorList.Count > 0 )
			{
				OnLoadError?.Invoke( ErrorList );
				IsRunning = false;
				return;
			}
			Engine?.Update();
			Routine?.Update();
		}

		protected void Cancel()
		{
			IsRunning = false;
			Engine?.Cancel();
			Routine.Clear();
			ErrorList.Clear();
		}

		/// <summary>
		/// エラー受信
		/// </summary>
		private void OnError(ResultCode code)
		{
			ErrorList.Add(code);
		}

	}
}

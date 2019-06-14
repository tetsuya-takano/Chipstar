using Chipstar.Downloads;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace Chipstar.Downloads.CriWare
{
	public interface ICriDownloader : IDisposable
	{
		Action<string, string> OnInstalled { set; }
		Action<ResultCode> OnError { set; }
		Func<IAccessPoint, string, IAccessLocation> GetFileDLLocation { set; }
		Func<string, long, bool> GetSuccessDL { set; }
		IEnumerator Init(IAccessPoint remotePath);
		ILoadProcess Start(IJobEngine engine, string relativePath, string fileVersion, long size );
	}
	/// <summary>
	/// ファイルDLに関するマネージャ
	/// </summary>
	public class CriDownloader : ICriDownloader
	{
		//=====================================
		//	変数
		//=====================================
		private IAccessPoint m_accessPoint = null;
		private IAccessPoint m_storage = null;
		private Dictionary<string, bool> m_dlRequestDict = new Dictionary<string, bool>();

		//=====================================
		//	プロパティ
		//=====================================
		public Func<string, string, bool> OnCheckVersion { private get; set; }
		public Action<string, string> OnInstalled { private get; set; }
		public Func<string, long, bool> GetSuccessDL { private get; set; }

		public Func<IAccessPoint, string, IAccessLocation> GetFileDLLocation { private get; set; }

		public Action<ResultCode> OnError { private get; set; }
		//=====================================
		//	関数
		//=====================================

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public CriDownloader(
			IAccessPoint storageFolder
			)
		{
			m_storage		= storageFolder	;
		}

		/// <summary>
		/// 破棄処理
		/// </summary>
		public void Dispose()
		{
			OnCheckVersion	= null;
			OnInstalled		= null;
			OnError = null;
		}

		/// <summary>
		/// 初期化
		/// </summary>
		public IEnumerator Init( IAccessPoint remotePath  )
		{
			m_dlRequestDict.Clear();
			m_accessPoint = remotePath;
			yield break;
		}

		/// <summary>
		/// DL開始
		/// </summary>
		public ILoadProcess Start( IJobEngine engine, string relativePath, string fileVersion, long size )
		{
			//	アクセス先
			var srcLocation = GetFileDLLocation?.Invoke( m_accessPoint, relativePath );
			//	現在進行系
			if( engine.HasRequest( srcLocation ) )
			{
				//	完了するまで待つ
				return new WaitLoadProcess( () => m_dlRequestDict[ srcLocation.AccessKey ] );
			}

			//	保存先
			var dstLocation	= m_storage.ToLocation( relativePath );
			//	同時リクエスト対応
			m_dlRequestDict[srcLocation.AccessKey] = false;

			var job = DoRequest(engine, srcLocation, dstLocation);
			return new LoadProcess<Empty>( 
				job	, 
				_ => 
			{
				if( GetSuccessDL?.Invoke( relativePath, size ) ?? false )
				{
					//	上書き
					OnInstalled?.Invoke( relativePath, fileVersion );
					//	リクエスト完了とする
					m_dlRequestDict[ srcLocation.AccessKey] = true;
				}
			}, onError: code => DoError( code ));
		}

		private void DoError(ResultCode code)
		{
			OnError?.Invoke( code );
		}

		/// <summary>
		/// リクエスト
		/// </summary>
		private ILoadJob<Empty> DoRequest( IJobEngine engine, IAccessLocation src, IAccessLocation dst )
		{
			var job = WRDL.GetFileDL(src, dst);
			engine.Enqueue( job );

			return job;
		}
	}
}
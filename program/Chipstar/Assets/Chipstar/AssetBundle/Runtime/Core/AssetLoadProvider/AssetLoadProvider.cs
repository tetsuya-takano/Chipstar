using Chipstar.Downloads;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Chipstar.Downloads
{
	/// <summary>
	/// アセット読み込みまわりの管理
	/// </summary>
	public interface IAssetLoadProvider : IDisposable
	{
		Action<ResultCode> OnError { set; }

		Action OnStartAny { set; }
		Action OnStopAny { set; }

		IAssetLoadOperation<T> LoadAsset<T>(string path) where T : UnityEngine.Object;
		IAssetLoadOperation<T> LoadAsset<T>(string path, Func<string, ILoadProcess>[] preProcess) where T : UnityEngine.Object;

		ISceneLoadOperation LoadLevel(string path, LoadSceneMode mode);
		ISceneLoadOperation LoadLevel(string path, LoadSceneMode mode, Func<string, ILoadProcess>[] preProcess);

		IPreloadOperation Preload( ILoadProcess process );

		void Cancel();
		void DoUpdate();
	}

	/// <summary>
	/// アセット読み込み統括
	/// </summary>
	public sealed class AssetLoadProvider : IAssetLoadProvider
	{
		//=======================
		//	プロパティ
		//=======================
		public Action<ResultCode> OnError { private get; set; }
		private IFactoryContainer Container { get; set; }
		private OperationRoutine  Routine { get; set; }
		public Action OnStopAny { private get; set; }
		public Action OnStartAny { private get; set; }

		//=======================
		//	関数
		//=======================

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public AssetLoadProvider( IFactoryContainer container )
		{
			Container = container;
			Routine = new OperationRoutine();
		}

		/// <summary>
		/// 破棄処理
		/// </summary>
		public void Dispose()
		{
			Container.Dispose();
			Container = null;
			OnError = null;
			OnStopAny = null;
			OnStartAny = null;
		}

		/// <summary>
		/// アセットの取得
		/// </summary>
		public IAssetLoadOperation<T> LoadAsset<T>( string path ) where T : UnityEngine.Object
		{
			var factory = Container.GetFromAsset( path );
			return AddCueue(factory.Create<T>(path));
		}

		/// <summary>
		/// DLつきAssetLoad
		/// </summary>
		public IAssetLoadOperation<T> LoadAsset<T>(string path, Func<string,ILoadProcess>[] preProcess ) where T : UnityEngine.Object
		{
			var factory = Container.GetFromAsset(path);
			// ダウンロードを待ってからロードする機能
			var operation = new DownloadAssetOperation<T>(
				path,
				preProcess, 
				factory.Create<T>( path )
			);
			return AddCueue(operation);
		}

		/// <summary>
		/// シーン遷移
		/// </summary>
		public ISceneLoadOperation LoadLevel( string path, LoadSceneMode mode )
		{
			var factory = Container.GetFromScene( path );
			return AddCueue(factory.Create(path, mode));
		}
		/// <summary>
		/// シーン加算
		/// </summary>
		public ISceneLoadOperation LoadLevel(string path, LoadSceneMode mode, Func<string, ILoadProcess>[] preProcess)
		{
			var factory = Container.GetFromScene( path );
			// ダウンロードを待ってからDLする機能
			var operation = new DownloadSceneOperation(
				path,
				preProcess,
				factory.Create(path, mode)
			);
			return AddCueue(operation);
		}
		/// <summary>
		/// 事前ロード用処理
		/// </summary>
		/// <param name="process"></param>
		/// <returns></returns>
		public IPreloadOperation Preload(ILoadProcess process)
		{
			var operation = new PreloadOperation(process);
			return AddCueue( operation );
		}

		/// <summary>
		/// 追加
		/// </summary>
		private T AddCueue<T>( T operation ) where T : ILoadOperater
		{
			operation.OnError = (code) => OnError?.Invoke( code );
			operation.OnStart = (_) => OnStartAny?.Invoke( );
			operation.OnStop = (_) => OnStopAny?.Invoke( );
			return Routine.Register(operation);
		}
		/// <summary>
		/// 更新
		/// </summary>
		public void DoUpdate()
		{
			Routine.Update();
		}

		/// <summary>
		/// 停止
		/// </summary>
		public void Cancel()
		{
			Routine.Clear();
		}
	}
}
#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Chipstar.Downloads
{
	/// <summary>
	/// エディタ用のアセットロード機能
	/// ResourcesとAssetDatabase
	/// </summary>
	public sealed class AssetLoadSimulator : IAssetLoadProvider
	{
		//=================================
		//	プロパティ
		//=================================
		private EditorFactoryContainer Container { get; set; }
		private OperationRoutine Routine { get; } = new OperationRoutine();
		public Action<ResultCode> OnError { set; private get; }
		public Action OnStopAny { set; private get; }
		public Action OnStartAny { set; private get; }

		//=================================
		//	関数
		//=================================

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public AssetLoadSimulator( string assetAccessPrefix )
		{
			Container	= new EditorFactoryContainer
				(
					afterLoginAssetFact : new EditorLoadAssetFactory( assetAccessPrefix ),
					alwaysAssetFact		: new ResourcesLoadFactory(1),
					afterLoginSceneFact	: new EditorSceneLoadFactory(),
					alwaysSceneFact		: new BuiltInSceneLoadFactory(1)
				);
		}

		/// <summary>
		/// 破棄処理
		/// </summary>
		public void Dispose()
		{
			Container.Dispose();
			Container = null;
			Routine.Clear();
			OnStartAny = null;
			OnStopAny = null;
		}
		/// <summary>
		/// アセット
		/// </summary>
		public IAssetLoadOperation<T> LoadAsset<T>( string path ) where T : UnityEngine.Object
		{
			var factory = Container .GetFromAsset( path );

			return Register(factory.Create<T>(path));
		}
		public IAssetLoadOperation<T> LoadAsset<T>(string path, Func<string, ILoadProcess>[] preProcess) where T : UnityEngine.Object
		{
			return LoadAsset<T>(path);
		}
		public ISceneLoadOperation LoadLevel(string path, LoadSceneMode mode)
		{
			var factory = Container.GetFromScene(path);
			return Register(factory.Create(path, mode));
		}

		public ISceneLoadOperation LoadLevel(string path, LoadSceneMode mode, Func<string, ILoadProcess>[] preProcess)
		{
			return LoadLevel(path, mode);
		}

		public IPreloadOperation Preload(ILoadProcess process)
		{
			return Register(new PreloadOperation(process));
		}

		private T Register<T>( T operation ) where T : ILoadOperater
		{
			operation.OnStart = _ => OnStartAny?.Invoke();
			operation.OnStop = _ => OnStopAny?.Invoke();

			return Routine.Register( operation );
		}

		/// <summary>
		/// ログイン状態を切り替える
		/// </summary>
		public void SetLoginMode( bool isLogin )
		{
			Container.SetLoginMode( isLogin );
		}

		public void Cancel()
		{
			Routine.Clear();
		}

		public void DoUpdate()
		{
			Routine.Update();
		}

	}
}
#endif
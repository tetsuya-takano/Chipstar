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
		IAssetLoadOperation<T>	LoadAsset<T>	 ( string path ) where T : UnityEngine.Object;
		ISceneLoadOperation		LoadLevel		 ( string path );
		ISceneLoadOperation		LoadLevelAdditive( string path );
		void Cancel();
		void DoUpdate();
	}

	/// <summary>
	/// アセット読み込み統括
	/// </summary>
	public class AssetLoadProvider : IAssetLoadProvider
	{
		//=======================
		//	変数
		//=======================
		private IFactoryContainer Container { get; set; }
		private OperationRoutine  Routine { get; set; }
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
			Routine.Clear();
		}

		/// <summary>
		/// アセットの取得
		/// </summary>
		public IAssetLoadOperation<T> LoadAsset<T>( string path ) where T : UnityEngine.Object
		{
			var factory = Container.GetFromAsset( path );
			Chipstar.Log_LoadAsset<T>( path, factory );
			return AddCueue(factory.Create<T>(path));
		}

		/// <summary>
		/// シーン遷移
		/// </summary>
		public ISceneLoadOperation LoadLevel( string path )
		{
			var factory = Container.GetFromScene( path );
			Chipstar.Log_LoadLevel( path, factory );
			return AddCueue(factory.LoadLevel(path));
		}
		/// <summary>
		/// シーン加算
		/// </summary>
		public ISceneLoadOperation LoadLevelAdditive( string path )
		{
			var factory = Container.GetFromScene( path );
			Chipstar.Log_LoadLevelAdditive( path, factory );
			return AddCueue(factory.LoadLevelAdditive(path));
		}
		/// <summary>
		/// 追加
		/// </summary>
		private T AddCueue<T>( T operation ) where T : ILoadOperation
		{
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
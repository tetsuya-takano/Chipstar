#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
					alwaysAssetFact		: new ResourcesLoadFactory(),
					afterLoginSceneFact	: new EditorSceneLoadFactory(),
					alwaysSceneFact		: new BuiltInSceneLoadFactory()
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
		}
		/// <summary>
		/// アセット
		/// </summary>
		public IAssetLoadOperation<T> LoadAsset<T>( string path ) where T : UnityEngine.Object
		{
			var factory = Container .GetFromAsset( path );

			Chipstar.Log_LoadAsset<T>( path, factory );

			return Routine.Register(factory.Create<T>(path));
		}

		/// <summary>
		/// シーン
		/// </summary>
		public ISceneLoadOperation LoadLevel( string path )
		{
			var factory = Container .GetFromScene( path );

			Chipstar.Log_LoadLevel( path, factory );

			return Routine.Register(factory.LoadLevel(path));
		}
		/// <summary>
		/// シーン加算
		/// </summary>
		public ISceneLoadOperation LoadLevelAdditive( string path )
		{
			var factory = Container .GetFromScene( path );

			Chipstar.Log_LoadLevelAdditive( path, factory );

			return Routine.Register(factory.LoadLevelAdditive(path));
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
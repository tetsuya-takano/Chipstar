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
		//
		//=================================
		//=================================
		//	プロパティ
		//=================================
		private IAssetLogger		Logger		{ get; set; }
		private IFactoryContainer	Container	{ get; set; }

		//=================================
		//	関数
		//=================================

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public AssetLoadSimulator()
		{
			Logger		= new AssetLogger();
			Container	= new FactoryContainer
				(
					new ResourcesLoadFactory(),
					new EditorLoadAssetFactory(),
					new EditorSceneLoadFactory()
				);
		}

		/// <summary>
		/// 破棄処理
		/// </summary>
		public void Dispose()
		{
			Container.Dispose();
			Container = null;
		}
		/// <summary>
		/// アセット
		/// </summary>
		public IAssetLoadOperation<T> LoadAsset<T>( string path ) where T : UnityEngine.Object
		{
			Logger.LoadAsset<T>( path );
			return Container
					.Get<IAssetLoadFactory>( path )
					.Create<T>( path );
		}

		/// <summary>
		/// シーン
		/// </summary>
		public ISceneLoadOperation LoadLevel( string path )
		{
			Logger.LoadLevel( path );
			return Container
					.Get<ISceneLoadFactory>( path )
					.LoadLevel( path );
		}
		/// <summary>
		/// シーン加算
		/// </summary>
		public ISceneLoadOperation LoadLevelAdditive( string path )
		{
			Logger.LoadLevelAdditive( path );
			return Container
					.Get<ISceneLoadFactory>( path )
					.LoadLevelAdditive( path );
		}
	}
}
#endif
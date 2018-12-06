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
		private IFactoryContainer	Container	{ get; set; }

		//=================================
		//	関数
		//=================================

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public AssetLoadSimulator( string assetAccessPrefix )
		{
			Container	= new FactoryContainer
				(
					assets : new IAssetLoadFactory[]
					{
						new EditorLoadAssetFactory( assetAccessPrefix ),
						new ResourcesLoadFactory()
					},
					scenes : new ISceneLoadFactory[]
					{
						new BuiltInSceneLoadFactory(),
						new EditorSceneLoadFactory(),
					}
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
			var factory = Container .GetFromAsset( path );

			Chipstar.Log_LoadAsset<T>( path, factory );

			return factory.Create<T>( path );
		}

		/// <summary>
		/// シーン
		/// </summary>
		public ISceneLoadOperation LoadLevel( string path )
		{
			var factory = Container .GetFromScene( path );

			Chipstar.Log_LoadLevel( path, factory );

			return factory.LoadLevel( path );
		}
		/// <summary>
		/// シーン加算
		/// </summary>
		public ISceneLoadOperation LoadLevelAdditive( string path )
		{
			var factory = Container .GetFromScene( path );

			Chipstar.Log_LoadLevelAdditive( path, factory );

			return factory.LoadLevelAdditive( path );
		}
	}
}
#endif
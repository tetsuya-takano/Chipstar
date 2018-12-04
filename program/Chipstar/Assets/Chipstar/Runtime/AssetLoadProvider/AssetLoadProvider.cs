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

		//=======================
		//	関数
		//=======================

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public AssetLoadProvider( IFactoryContainer container )
		{
			Container = container;
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
		/// アセットの取得
		/// </summary>
		public IAssetLoadOperation<T> LoadAsset<T>( string path ) where T : UnityEngine.Object
		{
			var factory = Container.Get<IAssetLoadFactory>( path );
			Debug.AssertFormat( factory != null, "Not Found Factory : {0}({1})", path, typeof( IAssetLoadFactory ).Name );

			Debug.LogFormat( "Load Asset ::: {0}", path );
			return factory.Create<T>( path );
		}

		/// <summary>
		/// シーン遷移
		/// </summary>
		public ISceneLoadOperation LoadLevel( string path )
		{
			var factory = Container.Get<ISceneLoadFactory>( path );
			Debug.AssertFormat( factory != null, "Not Found Factory : {0}({1})", path, typeof( ISceneLoadFactory ).Name );
			Debug.LogFormat( "Load Level ::: {0}", path );
			return factory.LoadLevel( path );
		}
		/// <summary>
		/// シーン加算
		/// </summary>
		public ISceneLoadOperation LoadLevelAdditive( string path )
		{
			var factory = Container.Get<ISceneLoadFactory>( path );
			Debug.AssertFormat( factory != null, "Not Found Factory : {0}({1})", path, typeof( ISceneLoadFactory ).Name );
			Debug.LogFormat( "Load Level Additive ::: {0}", path );
			return factory.LoadLevelAdditive( path );
		}
	}
}
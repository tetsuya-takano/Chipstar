using Chipstar.Downloads;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Chipstar.AssetLoad
{
	/// <summary>
	/// アセット読み込みまわりの管理
	/// </summary>
	public interface IAssetLoadProvider
	{
		ILoadOperation<T>	LoadAsset<T>	( string path ) where T : UnityEngine.Object;
		AsyncOperation		LoadLevel		( string path );
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
		/// 
		/// </summary>
		public ILoadOperation<T> LoadAsset<T>( string path ) where T : UnityEngine.Object
		{
			var factory = Container.Get<IAssetLoadFactory>( path );
			if( factory == null )
			{
				return null;
			}

			Debug.LogFormat( "Load Asset ::: {0}", path );
			return factory.Create<T>( path );
		}

		/// <summary>
		/// シーン遷移
		/// </summary>
		public AsyncOperation LoadLevel( string path )
		{
			var factory = Container.Get<ISceneLoadFactory>( path );
			if( factory == null )
			{
				return null;
			}
			Debug.LogFormat( "Load Level ::: {0}", path );
			return factory.LoadLevel( path );
		}
		/// <summary>
		/// シーン加算
		/// </summary>
		public AsyncOperation LoadLevelAdditive( string path )
		{
			var factory = Container.Get<ISceneLoadFactory>( path );
			if( factory == null )
			{
				return null;
			}
			Debug.LogFormat( "Load Level Additive ::: {0}", path );
			return factory.LoadLevelAdditive( path );
		}
	}
}
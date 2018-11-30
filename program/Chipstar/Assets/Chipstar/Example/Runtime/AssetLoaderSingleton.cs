using Chipstar.Downloads;
using System.Collections;
using System.IO;
using UnityEngine;

namespace Chipstar.Example
{
	/// <summary>
	/// とりあえず使い勝手を試すシングルトン
	/// </summary>
	public sealed class AssetLoaderSingleton : MonoBehaviour
	{
		//==================================
		//	static
		//==================================
		private static AssetLoaderSingleton I = null;

		//==================================
		//	変数
		//==================================
		private bool						m_isInit			= false;
		private IAssetManager               m_manager           = null;
		//==================================
		//	関数
		//==================================

		public static IEnumerator Wakeup()
		{
			if( I != null )
			{
				yield break;
			}
			var go = new GameObject( "AssetLoader(Singleton)" ).AddComponent<AssetLoaderSingleton>();
			DontDestroyOnLoad( go );
			yield return null;
		}


		public static IEnumerator Setup()
		{
			yield return Wakeup();
			if( I.m_isInit )
			{
				yield break;
			}
			yield return I.m_manager.Setup();
			I.m_isInit = true;
		}

		public static IEnumerator Preload( string path )
		{
			var operation = I.m_manager.Preload( path );
			yield return operation;
		}

		public static IAssetLoadOperation<T> LoadAsset<T>( string path ) where T : UnityEngine.Object
		{
			var operation = I.m_manager.LoadAsset<T>( path );
			return operation;
		}

		public static ISceneLoadOperation LoadLevel( string scenePath )
		{
			return I.m_manager.LoadLevel( scenePath );
		}

		private void Awake()
		{
			if( I != null)
			{
				DestroyObject( gameObject );
				return;
			}
			I = this;
		}

		private void OnDestroy()
		{
			if( I != this)
			{
				return;
			}
			I = null;

			m_manager.Dispose();
		}

		/// <summary>
		/// 初期化
		/// </summary>
		private void Start()
		{
			var config = new
			{
				ServerUrl    = Path.Combine( Application.dataPath, "../../build/windows/lz4/" ),
				BuildInfoFile= "buildMap.json",
				CacheStorage = Path.Combine( Application.dataPath, "../../cacheStorage/" ),
				LocalSaveFile= "localVersion.json"
			};
#if UNITY_EDITOR
			m_manager = AssetManager.Simulator();
#else
			m_manager = AssetManager.Default<RuntimeBundlleData>( config.ServerUrl, config.BuildInfoFile, config.CacheStorage, config.LocalSaveFile );
#endif
		}

		private void Update()
		{
			m_manager.DoUpdate();
		}
	}
}
using Chipstar.Downloads;
using System;
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
		//	関数 static
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

		/// <summary>
		/// 初期化関数
		/// １度しか行わない
		/// </summary>
		public static IEnumerator SetupOnlySingle()
		{
			yield return Wakeup();
			if( I.m_isInit )
			{
				yield break;
			}
			yield return I.m_manager.Setup();
			I.m_isInit = true;
		}

		/// <summary>
		/// 事前ロードのみ処理
		/// </summary>
		public static IEnumerator PreloadOnly( string path )
		{
			var operation = I.m_manager.Preload( path );
			yield return operation;
		}
		/// <summary>
		/// アセットの読み込みのみ処理
		/// </summary>
		public static IAssetLoadOperation<T> LoadAssetWithoutDownload<T>( string path ) where T : UnityEngine.Object
		{
			var operation = I.m_manager.LoadAsset<T>( path );
			return operation;
		}
		public IDisposable LoadAsset<T>( string path, Action<T> onLoaded )
			where T : UnityEngine.Object
		{
			var refCounter = m_manager.CreateAssetReference( path );
			StartCoroutine( DoDownloadWithAssetLoad<T>( path, onLoaded ) );
			return refCounter;
		}

		/// <summary>
		/// シーン遷移のみ処理
		/// </summary>
		public static ISceneLoadOperation LoadLevelWithoutDownload( string scenePath )
		{
			return I.m_manager.LoadLevel( scenePath );
		}

		//==================================
		//	関数 
		//==================================
		private IEnumerator DoDownloadWithAssetLoad<T>( string path, Action<T> onLoaded )
			where T : UnityEngine.Object
		{
			var preload  = m_manager.Preload( path );
			yield return preload;
			var loadAsset= m_manager.LoadAsset<T>( path );
			yield return loadAsset;

			onLoaded( loadAsset.Content );
		}

		//==================================
		//	関数 unity
		//==================================

		private void Awake()
		{
			if( I != null)
			{
				DestroyObject( gameObject );
				return;
			}
			I = this;
		}

		/// <summary>
		/// 破棄
		/// </summary>
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
#if false
			m_manager = AssetManager.Simulator();
#else
			m_manager = AssetManager.Default<RuntimeBundleData>( config.ServerUrl, config.BuildInfoFile, config.CacheStorage, config.LocalSaveFile );
#endif
		}

		/// <summary>
		/// 
		/// </summary>
		private void Update()
		{
			m_manager.DoUpdate();
		}

	}
}
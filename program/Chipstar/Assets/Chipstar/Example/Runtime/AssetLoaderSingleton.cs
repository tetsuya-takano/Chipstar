using Chipstar.Downloads;
using Chipstar.Downloads;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;
using System.Text;

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
		private bool								m_isInit			= false;
		private ILoadDatabase<RuntimeBundlleData>   m_database			= null;
		private IAssetLoadProvider					m_assetLoadProvider = null;
		private IAssetBundleLoadProvider			m_downloadProvider  = null;
		private StringBuilder                       m_builder			= new StringBuilder();
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
			yield return I.m_downloadProvider.InitLoad();
			I.m_isInit = true;
		}

		public static IEnumerator Preload( string path )
		{
			var operation = I.m_downloadProvider.Load( path ).ToYieldInstruction();
			yield return operation;
		}

		public static IAssetLoadOperation<T> LoadAsset<T>( string path ) where T : UnityEngine.Object
		{
			var operation = I.m_assetLoadProvider.LoadAsset<T>( path );
			return operation;
		}

		public static ISceneLoadOperation LoadLevel( string scenePath )
		{
			return I.m_assetLoadProvider.LoadLevel( scenePath );
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

			m_database.Dispose();
			m_downloadProvider	.Dispose();
			m_assetLoadProvider	.Dispose();
		}

		/// <summary>
		/// 初期化
		/// </summary>
		private void Start()
		{
			//	接続先
			var acccesPoint   = new EntryPoint( Path.Combine( Application.dataPath, "../../build/windows/lz4/" ) );
			//	保存先
			var cacheStorage  = new EntryPoint( Path.Combine( Application.dataPath, "../../cacheStorage/" ) );

			//	コンテンツカタログ
			var loadDatabase = new LoadDatabase<BuildMapDataTable, BundleBuildData, AssetBuildData, RuntimeBundlleData>( acccesPoint,"buildMap.json" );
			//	ローカル保存データベース
			var cacheDatabase= new CacheDatabase( cacheStorage, "localVersion.json"  );

			//	リクエスト作成
			var creator      = new SampleJobCreator();
			var jobEngine    = new JobEngine();

			//	DL機能を統合
			m_downloadProvider = new AssetBundleLoadProvider<RuntimeBundlleData>
				(
					loadDatabase: loadDatabase,
					cacheDatabase: cacheDatabase,
					jobCreator: creator,
					dlEngine: jobEngine
				);

			//	読み込みリクエスト側機能
			var factoryContainer = new FactoryContainer
			(
				new AssetBundleLoadFactory<RuntimeBundlleData>( loadDatabase ),
				new ResourcesLoadFactory(),
				new SceneLoadFactory<RuntimeBundlleData>( loadDatabase )
			);
			//	統合
			m_assetLoadProvider = new AssetLoadProvider
				(
					factoryContainer
				);

			m_database = loadDatabase;
		}

		private void Update()
		{
			m_downloadProvider.DoUpdate();
		}


		private void OnGUI()
		{
			if( !m_isInit ) { return; }
			m_builder.Length = 0;
			var defaultColor = GUI.contentColor;
			GUI.contentColor = Color.red;
			foreach( var bundle in m_database.BundleList )
			{
				m_builder
					.AppendFormat( "{0} : {1}", bundle.Name, bundle.RefCount )
					.AppendLine();
			}
			GUI.contentColor = defaultColor;
			GUILayout.Label( m_builder.ToString(),GUI.skin.button );
		}
	}
}
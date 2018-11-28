using UnityEngine;
using System.Collections;
using Chipstar.Downloads;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using System.IO;
using Chipstar.AssetLoad;

namespace Chipstar.Example
{
    public class AssetLoadExample : MonoBehaviour
    {
		//========================================
		//	SerializeField
		//========================================
		[SerializeField] RawImage	m_image			= null;
		[SerializeField] Text		m_dlListText	= null;
		[SerializeField] Text		m_cacheListText	= null;

		//========================================
		//	変数
		//========================================
		private IDisposable					m_disposable = null;
		private IAssetBundleLoadProvider	m_downloadProvider	 = null;
		private IAssetLoadProvider          m_assetLoadProvider  = null;

		//========================================
		//	関数
		//========================================
		
		// Use this for initialization
		IEnumerator Start()
        {
			//	接続先
			var acccesPoint   = new EntryPoint( Path.Combine( Application.dataPath, "../../build/windows/lzma/" ) );
			//	保存先
			var cacheStorage  = new EntryPoint( Path.Combine( Application.dataPath, "../../cacheStorage/" ) );
			
			//	コンテンツカタログ
			var loadDatabase = new LoadDatabase<BuildMapDataTable, BundleBuildData, AssetBuildData, RuntimeBundlleData>( acccesPoint,"buildMap.json" );
			//	ローカル保存データベース
			var cacheDatabase= new CacheDatabase( cacheStorage, "localVersion.json"  );

			m_disposable = loadDatabase;

			//	リクエスト作成
			var creator      = new SampleJobCreator();
            var jobEngine    = new JobEngine();

			//	DL機能を統合
            m_downloadProvider = new AssetBundleLoadProvider<RuntimeBundlleData>
                (
                    loadDatabase	: loadDatabase,
					cacheDatabase	: cacheDatabase,
                    jobCreator		: creator,
                    dlEngine		: jobEngine
                );

			//	読み込みリクエスト側機能
			var factoryContainer = new FactoryContainer
			(
				new AssetBundleLoadFactory<RuntimeBundlleData>( loadDatabase ),
				new ResourcesLoadFactory()
			);
			//	統合
			m_assetLoadProvider = new AssetLoadProvider( factoryContainer );

            yield return null;

			//	初期化開始
            yield return m_downloadProvider.InitLoad( );

			m_cacheListText	.text = cacheDatabase.ToString();
			m_dlListText	.text = loadDatabase .ToString();

			//	アセットバンドルDL
			var path = "Assets/BundleTarget/Container 1.prefab";
			var downloadTask = m_downloadProvider.Load( path );
            yield return new WaitWhile(() => !downloadTask.IsCompleted );
			yield return null;
			//	リソースロード
			var operation = m_assetLoadProvider.LoadAsset<GameObject>( path );
			yield return operation;
			var prefab		= operation.Content;
			var container	= prefab.GetComponent<Container>();
			var parent		= m_image.transform.parent;
			foreach( var item in container.List )
			{
				var img = Instantiate(m_image, parent);
				img.texture = item as Texture;
			}

			//	保存
			cacheDatabase.Apply();
		}
        private void OnDestroy()
        {
			m_disposable.Dispose();
        }

        private void Update()
        {
            if( m_downloadProvider == null ) { return; }
            m_downloadProvider.DoUpdate();
        }
    }
}
using UnityEngine;
using System.Collections;
using Chipstar.Downloads;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using System.IO;
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
		private IAssetLoadProvider m_provider = null;

		//========================================
		//	関数
		//========================================
		
		// Use this for initialization
		IEnumerator Start()
        {
			//  https://www.google.co.jp/images/branding/googlelogo/2x/googlelogo_color_272x92dp.png
			var acccesPoint   = new EntryPoint( Path.Combine( Application.dataPath, "../../build/windows/lz4/" ) );
			var cacheStorage  = new EntryPoint( Path.Combine( Application.dataPath, "../../cacheStorage/" ) );

			var loadDatabase = new LoadDatabase<BuildMapDataTable, BundleBuildData, AssetBuildData, RuntimeBundlleData>( acccesPoint,"buildMap.json" );
			var cacheDatabase= new CacheDatabase( cacheStorage, "localVersion.json"  );

			var creator      = new SampleJobCreator<RuntimeBundlleData>();
            var jobEngine    = new JobEngine();

            m_provider = new AssetLoadProvider<RuntimeBundlleData>
                (
                    loadDatabase	: loadDatabase,
					cacheDatabase	: cacheDatabase,
                    jobCreator		: creator,
                    dlEngine		: jobEngine
                );

            yield return null;

			//	初期化開始
            yield return m_provider.InitLoad( );

			m_cacheListText	.text = cacheDatabase.ToString();
			m_dlListText	.text = loadDatabase .ToString();


			//	完了
			var path = "Assets/BundleTarget/Container 1.prefab";
			var downloadTask = m_provider.Load( path );
            yield return new WaitWhile(() => !downloadTask.IsCompleted );
			yield return null;
			var assetData	= loadDatabase.Find( path );
			var bundle      = assetData.BundleData;
			var operation   = bundle.LoadAsync( path );
			yield return new WaitWhile( () => !operation.isDone );

			var prefab		= operation.asset as GameObject;
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
        }

        private void Update()
        {
            if( m_provider == null ) { return; }
            m_provider.DoUpdate();
        }
    }
}
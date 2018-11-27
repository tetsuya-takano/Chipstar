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
        [SerializeField]
        RawImage m_image = null;
        private IAssetLoadProvider m_provider = null;
        private IDisposable        m_loadDispose = null;
        // Use this for initialization
        IEnumerator Start()
        {
			//  https://www.google.co.jp/images/branding/googlelogo/2x/googlelogo_color_272x92dp.png
			var acccesPoint   = new EntryPoint( Path.Combine( Application.dataPath, "../../build/windows/lz4" ) );
			var cacheStorage  = new EntryPoint( Path.Combine( Application.dataPath, "../../cacheStorage/" ) );

			var loadDatabase = new LoadDatabase<BuildMapDataTable, BundleBuildData, AssetBuildData, RuntimeBundlleData>( acccesPoint );
			var cacheDatabase= new CacheDatabase( cacheStorage );

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

            yield return m_provider.InitLoad( "buildMap.json", "localVersion.json" );
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
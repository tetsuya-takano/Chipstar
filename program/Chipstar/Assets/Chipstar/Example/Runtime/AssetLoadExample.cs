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

            var creator     = new SampleJobCreator<RuntimeBundlleData>();
            var database    = new LoadDatabase<BuildMapDataTable, BundleBuildData, AssetBuildData, RuntimeBundlleData>();
            var jobEngine   = new JobEngine();
            var acccesPoint = new AccessPoint( Path.Combine( Application.dataPath, "../../build/windows/" ) );

            m_provider = new AssetLoadProvider<RuntimeBundlleData>
                (
                    accessPoint: acccesPoint,
                    database: database,
                    jobCreator: creator,
                    dlEngine: jobEngine
                );

            yield return null;

            yield return m_provider.InitLoad( "buildMap.json" );


            m_loadDispose = m_provider.LoadAsset<GameObject>( "Assets/BundleTarget/Container 1.prefab", prefab =>
             {
                 var container = prefab.GetComponent<Container>();
                 var parent = m_image.transform.parent;
                 foreach( var item in container.List )
                 {
                     var img = Instantiate(m_image, parent);
                     img.texture = item as Texture;
                 }
             } );
        }
        private void OnDestroy()
        {
            m_loadDispose.Dispose();
        }

        private void Update()
        {
            if( m_provider == null ) { return; }
            m_provider.DoUpdate();
        }
    }
}
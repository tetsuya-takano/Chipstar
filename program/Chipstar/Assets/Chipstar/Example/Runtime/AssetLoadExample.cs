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
            var acccesPoint = new EntryPoint( Path.Combine( Application.dataPath, "../../build/windows/lz4" ) );

            m_provider = new AssetLoadProvider<RuntimeBundlleData>
                (
                    accessPoint: acccesPoint,
                    database: database,
                    jobCreator: creator,
                    dlEngine: jobEngine
                );

            yield return null;

            yield return m_provider.InitLoad( "buildMap.json" );
            var downloadTask = m_provider.Load("Assets/BundleTarget/Container 1.prefab" );
            yield return new WaitWhile(() => downloadTask.IsCompleted );

            //downloadTask.OnCompleted = () => 
            //{
            //     var prefab = downloadTask.Content;
            //     var container = prefab.GetComponent<Container>();
            //     var parent = m_image.transform.parent;
            //     foreach( var item in container.List )
            //     {
            //         var img = Instantiate(m_image, parent);
            //         img.texture = item as Texture;
            //     }
            //};
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
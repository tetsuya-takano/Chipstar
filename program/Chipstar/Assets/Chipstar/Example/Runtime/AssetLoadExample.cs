using UnityEngine;
using System.Collections;
using Chipstar.Downloads;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections.Generic;

public class AssetLoadExample : MonoBehaviour
{
    [SerializeField] RawImage m_image = null;
    IAssetLoadProvider m_provider = null;
    // Use this for initialization
    IEnumerator Start()
    {
        //  https://www.google.co.jp/images/branding/googlelogo/2x/googlelogo_color_272x92dp.png

        var creator = new JobCreator
            (
                onBytesLoad : location => new WWWDLJob<byte[]>      ( location, new WWWDL.BytesDL()),
                onTextLoad  : location => new WWWDLJob<string>      ( location, new WWWDL.TextDL ()),
                onBundleLoad: location => new WWWDLJob<AssetBundle> ( location, new WWWDL.AssetBundleDL()),
                onAssetLoad : null
            );
        var database    = new LoadDatabase<BuildMapDataTable, BundleBuildData, AssetBuildData, RuntimeBundlleData>();
        var jobEngine   = new JobEngine();


        m_provider = new AssetLoadProvider<RuntimeBundlleData>
            (
                database    : database,
                jobCreator  : creator,
                dlEngine    : jobEngine
            );

        yield return null;

        var contentLocation = new UrlLocation(@"D:\Programming\Unity\Chipstar\program\build\windows\buildMap.json");
        yield return m_provider.InitLoad( contentLocation );


        m_provider.LoadAsset<Texture>("Assets/BundleTarget/Square 3.png", texture =>
        {
            m_image.texture = texture;
        });
    }

    private void Update()
    {
        if (m_provider == null) { return; }
        m_provider.DoUpdate();
    }
}

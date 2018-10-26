using UnityEngine;
using System.Collections;
using Chipstar.Downloads;
using UnityEngine.Networking;

public class AssetLoadExample : MonoBehaviour
{
    IDownloadEngine engine = null;
    // Use this for initialization
    void Start()
    {
        engine  = new DownloadEngine();
        var request = new WWWDLJob<WWWDL.TextDL, string >("https://www.google.com/");
        engine.Enqueue( request );
        request.OnLoaded = text =>
        {
            Debug.Log(text);
        };
    }


    private void Update()
    {
        engine.Update();
    }
}

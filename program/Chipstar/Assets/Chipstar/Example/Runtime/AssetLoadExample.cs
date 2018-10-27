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

        for (int i = 0; i < 10; i++)
        {
            var request = new WWWDLJob<string>( "https://www.google.com/", new WWWDL.TextDL() );
            var idx = i + 1;
            request.OnLoaded = text =>
            {
                Debug.Log( idx + " ::::::::::::::: "+ text);
            };

            engine.Enqueue(request);
        }
    }


    private void Update()
    {
        engine.Update();
    }
}

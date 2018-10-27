using UnityEngine;
using System.Collections;
using Chipstar.Downloads;
using UnityEngine.Networking;
using UnityEngine.UI;

public class AssetLoadExample : MonoBehaviour
{
    [SerializeField] RawImage m_image = null;
    // Use this for initialization
    void Start()
    {

        //  https://www.google.co.jp/images/branding/googlelogo/2x/googlelogo_color_272x92dp.png

        var task = AssetLoader.Load<Texture>("");

        task.OnLoaded = texture =>
        {

        };

        task.Dispose();
    }
}

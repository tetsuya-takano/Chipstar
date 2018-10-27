using UnityEngine;
using System.Collections;
using Chipstar.Downloads;
using UnityEngine.Networking;
using UnityEngine.UI;

public class AssetLoadExample : MonoBehaviour
{
    [SerializeField] RawImage m_image = null;
    ILoadManager manager = null;
    // Use this for initialization
    void Start()
    {

        //  https://www.google.co.jp/images/branding/googlelogo/2x/googlelogo_color_272x92dp.png

        manager = new LoadManager();

        var task = manager.LoadAsset<Texture>(null);
        task.OnLoaded = texture =>
        {
            m_image.texture = texture;
        };
    }


    private void Update()
    {
        manager.Update();
    }
}

using UnityEngine;
using System.Collections;
using Chipstar.Downloads;

public class AssetLoadExample : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        var assetLoadTask = AssetLoader.Load<Texture>("");
        var sceneLoadTask = SceneLoader.LoadLevel("");

        assetLoadTask.OnLoaded = texture    => { };
        sceneLoadTask.OnLoaded = ()         => { };

        if (assetLoadTask.IsCompleted && sceneLoadTask.IsCompleted)
        {
            return;
        }
        assetLoadTask.Dispose();
        sceneLoadTask.Dispose();
    }
}

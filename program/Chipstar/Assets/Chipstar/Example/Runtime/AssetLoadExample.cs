using UnityEngine;
using System.Collections;
using Chipstar.Downloads;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using System.IO;
using Chipstar.AssetLoad;

namespace Chipstar.Example
{
    public class AssetLoadExample : MonoBehaviour
    {
		//========================================
		//	SerializeField
		//========================================
		[SerializeField] RawImage	m_image			= null;
		[SerializeField] RawImage	m_loadedImage	= null;
		[SerializeField] Text		m_dlListText	= null;
		[SerializeField] Text		m_cacheListText	= null;


		[SerializeField] private Button m_loadLevelButton	= null;
		[SerializeField] private string m_scenePath			= null;

		//========================================
		//	変数
		//========================================

		//========================================
		//	関数
		//========================================
		
		// Use this for initialization
		IEnumerator Start()
        {
            yield return null;

			//	初期化開始
            yield return AssetLoaderSingleton.Setup( );

			//	アセットバンドルDL
			var path = "Assets/BundleTarget/Container 1.prefab";
            yield return AssetLoaderSingleton.Preload( path );
			yield return null;
			//	リソースロード
			var operation = AssetLoaderSingleton.LoadAsset<GameObject>( path );
			yield return operation;

			var prefab		= operation.Content;
			var container	= prefab.GetComponent<Container>();
			var parent		= m_image.transform.parent;
			foreach( var item in container.List )
			{
				var img = Instantiate(m_image, parent);
				img.texture = item as Texture;
			}
			var nextLoadOperate = AssetLoaderSingleton.LoadAsset<Texture>( "Assets/Resources/Square 6.png" );
			m_loadedImage.texture = nextLoadOperate.Content;
			m_loadLevelButton.onClick.AddListener( () => StartCoroutine( LoadLevel() ));


			operation.Dispose();
			nextLoadOperate.Dispose();
		}

		private IEnumerator LoadLevel()
		{
			yield return AssetLoaderSingleton.Preload( m_scenePath );

			var operation = AssetLoaderSingleton.LoadLevel( m_scenePath );
			yield return operation;
		}

		private void OnDestroy()
        {
        }
    }
}
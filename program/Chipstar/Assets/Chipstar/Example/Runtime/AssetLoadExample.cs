using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

namespace Chipstar.Example
{
	public class AssetLoadExample : MonoBehaviour
    {
		//========================================
		//	SerializeField
		//========================================
		[SerializeField] RawImage	m_image			= null;
		[SerializeField] RawImage	m_loadedImage	= null;

		[SerializeField] private Button m_loadLevelButton	= null;
		[SerializeField] private string m_scenePath			= null;

		//========================================
		//	変数
		//========================================

		private List<IDisposable> m_disposes = new List<IDisposable>();

		//========================================
		//	関数
		//========================================
		
		// Use this for initialization
		IEnumerator Start()
        {
			Debug.Log( "	初期化開始" );

			yield return AssetLoaderSingleton.SetupOnlySingle( );

			Debug.Log( "アセットバンドルDL" );
			var path = "Assets/BundleTarget/Container 1.prefab";
            yield return AssetLoaderSingleton.PreloadOnly( path );
			yield return null;
			Debug.Log( "リソースロード" );
			var operation = AssetLoaderSingleton.LoadAssetWithoutDownload<GameObject>( path );
			yield return operation;

			Debug.Log( "UI構築" );
			var prefab		= operation.Content;
			var container	= prefab.GetComponent<Container>();
			var parent		= m_image.transform.parent;
			foreach( var item in container.List )
			{
				var img = Instantiate(m_image, parent);
				img.texture = item as Texture;
			}
			var nextLoadOperate = AssetLoaderSingleton.LoadAssetWithoutDownload<Texture>( "Assets/Resources/Square 6.png" );
			m_loadedImage.texture = nextLoadOperate.Content;
			m_loadLevelButton.onClick.AddListener( () => StartCoroutine( LoadLevel() ));


			m_disposes.Add( operation );
			m_disposes.Add( nextLoadOperate );
		}

		private IEnumerator LoadLevel()
		{
			yield return AssetLoaderSingleton.PreloadOnly( m_scenePath );

			var operation = AssetLoaderSingleton.LoadLevelWithoutDownload( m_scenePath );
			yield return operation;
			m_disposes.Add( operation );
		}

		private void OnDestroy()
        {
			foreach( var d in m_disposes)
			{
				d.Dispose();
			}
        }
    }
}
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using Pony;

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

            yield return Loader.
            yield break;
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
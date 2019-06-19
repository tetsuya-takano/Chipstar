using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Chipstar.Downloads
{
	/// <summary>
	/// バンドルシーン
	/// </summary>
	public sealed class AssetBundleSceneLoadOperation<T> 
		: SceneLoadOperation
		where T : IRuntimeBundleData<T>
		
	{
		//======================================
		//	変数
		//======================================
		private AssetData<T> m_data = null;

		//======================================
		//	関数
		//======================================

		public AssetBundleSceneLoadOperation(AssetData<T> data, LoadSceneMode mode) : base(mode)
		{
			m_data = data;
			m_data?.AddRef();
		}

		protected override void DoDispose()
		{
			m_data?.ReleaseRef();
			ChipstarLog.Log_Dump_RefLog(m_data);
			m_data = null;
			base.DoDispose();
		}

		protected override AsyncOperation CreateLoadSceneAsync()
		{
			return SceneManager.LoadSceneAsync(m_data.Path, SceneMode);
		}

		public override string ToString()
		{
			return "[AssetBundleSceneLoadOperation]" + (m_data?.Path ?? string.Empty);
		}
	}
}
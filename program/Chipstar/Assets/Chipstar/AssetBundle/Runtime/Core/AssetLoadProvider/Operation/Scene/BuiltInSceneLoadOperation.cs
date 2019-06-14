using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Chipstar.Downloads
{
	/// <summary>
	/// 内包シーン
	/// </summary>
	public sealed class BuiltInSceneLoadOperation : SceneLoadOperation
	{
		//=======================================
		//	変数
		//=======================================
		private string m_path = string.Empty;

		//=======================================
		//	関数
		//=======================================
		public BuiltInSceneLoadOperation(string path, LoadSceneMode mode)
			: base(mode)
		{
			m_path = path;
		}

		protected override AsyncOperation CreateLoadSceneAsync()
		{
			return SceneManager.LoadSceneAsync(m_path, SceneMode);
		}

		protected override void DoComplete()
		{
			base.DoComplete();
			// 完了時に自動で破棄
			this.DisposeIfNotNull();
		}
	}
}
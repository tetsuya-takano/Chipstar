#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Chipstar.Downloads
{
	/// <summary>
	/// 
	/// </summary>
	public sealed class EditorSceneLoadOperation
		: LoadOperation, ISceneLoadOperater
	{
		//===============================
		//	変数
		//===============================
		private string m_path = string.Empty;
		private LoadSceneMode m_mode = LoadSceneMode.Single;
		private AsyncOperation m_request = null;
		private Action m_onComplete = null;

		public Action OnCompleted { set => m_onComplete = value; }

		//===============================
		//	プロパティ
		//===============================

		//===============================
		//	関数
		//===============================

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public EditorSceneLoadOperation(string path, LoadSceneMode mode)
		{
			m_path = path;
			m_mode = mode;
		}
		protected override void DoDispose()
		{
			m_onComplete = null;
			m_request = null;
			base.DoDispose();
		}

		protected override void DoRun()
		{
			var param = new LoadSceneParameters(m_mode);
			m_request = EditorSceneManager.LoadSceneAsyncInPlayMode(m_path, param);
		}

		protected override void DoComplete()
		{
			ChipstarUtils.OnceInvoke(ref m_onComplete);
		}

		protected override float GetProgress()
		{
			return m_request.progress;
		}

		protected override bool GetComplete()
		{
			return m_request.isDone;
		}
	}
}
#endif
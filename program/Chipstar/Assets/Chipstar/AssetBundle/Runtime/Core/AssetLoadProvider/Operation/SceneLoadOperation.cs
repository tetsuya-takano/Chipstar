using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Chipstar.Downloads
{
	/// <summary>
	/// シーン読み込み処理
	/// </summary>
	public interface ISceneLoadOperation
		: ILoadOperation
	{
		Action OnCompleted { set; }
	}
	public interface ISceneLoadOperater
		: ILoadOperater, ISceneLoadOperation
	{
	}
	/// <summary>
	/// シーン読み込みタスク
	/// </summary>
	public abstract class SceneLoadOperation 
		:	LoadOperation,
			ISceneLoadOperater
	{
		//==============================
		//	変数
		//==============================
		
		private AsyncOperation	m_sceneOperation = null;
		private Action m_onComplete = null;
		//==============================
		//	プロパティ
		//==============================
		public Action OnCompleted { set => m_onComplete = value; }
		protected LoadSceneMode SceneMode { get; private set; }
		//==============================
		//	関数
		//==============================

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public SceneLoadOperation( LoadSceneMode mode )
		{
			SceneMode = mode;
		}
		protected override void DoDispose()
		{
			OnCompleted = null;
			m_sceneOperation = null;
			base.DoDispose();
		}

		protected override void DoRun()
		{
			m_sceneOperation = CreateLoadSceneAsync();
		}

		protected abstract AsyncOperation CreateLoadSceneAsync();

		protected override void DoComplete()
		{
			ChipstarUtils.OnceInvoke(ref m_onComplete);
		}

		protected override float GetProgress()
		{
			return m_sceneOperation.progress;
		}

		protected override bool GetComplete()
		{
			return m_sceneOperation.isDone;
		}
	}
}
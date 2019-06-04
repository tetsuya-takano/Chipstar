using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Chipstar.Downloads
{
	/// <summary>
	/// シーン読み込みタスク
	/// </summary>
	public sealed class SceneLoadOperation 
		:	LoadOperation,
			ISceneLoadOperation
	{
		//==============================
		//	変数
		//==============================
		private AsyncOperation	m_sceneOperation = null;
		//==============================
		//	プロパティ
		//==============================
		public override bool IsCompleted { get { return m_sceneOperation != null && m_sceneOperation.isDone; } }
		public Action OnCompleted { set; private get; }

		//==============================
		//	関数
		//==============================

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public SceneLoadOperation( AsyncOperation operation )
		{
			m_sceneOperation = operation;
		}
		protected override void DoDispose()
		{
			OnCompleted = null;
			m_sceneOperation = null;
			base.DoDispose();
		}

		protected override void DoComplete()
		{
			var onComplete = OnCompleted;
			OnCompleted = null;
			onComplete?.Invoke();
		}
	}
}
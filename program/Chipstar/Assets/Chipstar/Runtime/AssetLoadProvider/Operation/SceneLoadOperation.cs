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
		public override bool keepWaiting { get { return !m_sceneOperation.isDone; } }

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
			m_sceneOperation = null;
		}
	}
}
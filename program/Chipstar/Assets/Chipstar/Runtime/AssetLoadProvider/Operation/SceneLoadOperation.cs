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
		private IDisposable     m_unloader  = null;
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
		public SceneLoadOperation( AsyncOperation operation ) : this( operation, null )
		{
			
		}
		public SceneLoadOperation( AsyncOperation operation, IDisposable unloadDispose )
		{
			m_sceneOperation = operation;
			m_unloader  = unloadDispose;
		}
		protected override void DoDispose()
		{
			m_sceneOperation = null;
			if( m_unloader != null )
			{
				m_unloader.Dispose();
			}
			m_unloader = null;
		}
	}
}
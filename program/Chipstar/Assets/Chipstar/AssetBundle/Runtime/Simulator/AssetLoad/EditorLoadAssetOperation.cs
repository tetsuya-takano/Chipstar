#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Chipstar.Downloads
{
	/// <summary>
	/// 
	/// </summary>
	public sealed class EditorLoadAssetOperation<T> 
		: AssetLoadOperation<T>
		where T : UnityEngine.Object
	{
		//===============================
		//	変数
		//===============================
		private T   m_asset     = null;
		private int m_waitFrame = 0;
		private int m_frameCount = 0;

		//===============================
		//	プロパティ
		//===============================
		public override T		Content		{ get { return m_asset	; } }
		public override bool	IsCompleted { get { return m_frameCount > m_waitFrame;} }

		//===============================
		//	関数
		//===============================

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public EditorLoadAssetOperation( string path, int waitFrame )
		{
			m_frameCount = 0;
			m_waitFrame = waitFrame;
			m_asset = AssetDatabase.LoadAssetAtPath<T>( path );
		}
		protected override void DoUpdate()
		{
			m_frameCount++;
		}

		/// <summary>
		/// 破棄
		/// </summary>
		protected override void DoDispose()
		{
			m_asset = null;
		}
	}
}
#endif
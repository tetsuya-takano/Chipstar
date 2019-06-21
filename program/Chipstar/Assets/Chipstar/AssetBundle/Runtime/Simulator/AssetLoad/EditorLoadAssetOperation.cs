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
		private string m_path = string.Empty;
		private int m_waitFrame = 0;
		private int m_frameCount = 0;
		private T m_asset = null;

		//===============================
		//	プロパティ
		//===============================
		//===============================
		//	関数
		//===============================

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public EditorLoadAssetOperation( string path, int waitFrame )
		{
			m_path = path;
			m_frameCount = 0;
			m_waitFrame = waitFrame;
		}
		protected override void DoPreUpdate()
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

		protected override float GetProgress()
		{
			return Mathf.InverseLerp(0, m_waitFrame, m_frameCount);
		}

		protected override T GetContent()
		{
			return m_asset;
		}

		protected override bool GetComplete()
		{
			return m_frameCount >= m_waitFrame;
		}

		protected override void DoComplete()
		{
			base.DoComplete();
		}

		protected override void DoRun()
		{
			m_frameCount = 0;
			m_asset = AssetDatabase.LoadAssetAtPath<T>(m_path);
		}
	}
}
#endif
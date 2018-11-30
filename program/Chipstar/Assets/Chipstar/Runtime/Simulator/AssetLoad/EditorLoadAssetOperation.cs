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
		private T m_asset = null;

		//===============================
		//	プロパティ
		//===============================
		public override T		Content		{ get { return m_asset	; } }
		public override bool	keepWaiting { get { return false	; } }

		//===============================
		//	関数
		//===============================

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public EditorLoadAssetOperation( string path )
		{
			m_asset = AssetDatabase.LoadAssetAtPath<T>( path );
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
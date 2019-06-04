using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Chipstar.Downloads
{
	/// <summary>
	/// Resources.LoadAsyc用
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public sealed class ResourcesLoadOperation<T> 
		: AssetLoadOperation<T> 
		where T : UnityEngine.Object
	{
		//===============================
		//	変数
		//===============================
		private ResourceRequest m_request = null;

		//===============================
		//	プロパティ
		//===============================
		public override bool IsCompleted { get { return m_request != null && m_request.isDone; } }
		public override T Content { get { return m_request != null ? m_request.asset as T : null; } }

		//===============================
		//	関数
		//===============================
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ResourcesLoadOperation( ResourceRequest request )
		{
			m_request = request;
		}

		/// <summary>
		/// 破棄
		/// </summary>
		protected override void DoDispose()
		{
			m_request = null;
		}
	}
}
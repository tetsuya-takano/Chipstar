using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Chipstar.Downloads
{
	/// <summary>
	/// アセットバンドルを撮ってくる機能
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public sealed class AssetBundleLoadOperation<T> : AssetLoadOperation<T>
		where T : UnityEngine.Object
	{
		//====================================
		//	変数
		//====================================
		private AssetBundleRequest m_request = null;
		//====================================
		//	プロパティ
		//====================================
		public override bool IsCompleted => m_request != null && m_request.isDone;
		public override T Content { get { return m_request == null ? null : m_request.asset as T; } }
		//====================================
		//	関数
		//====================================

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public AssetBundleLoadOperation( AssetBundleRequest request )
		{
			m_request = request;
		}

		/// <summary>
		/// 破棄処理
		/// </summary>
		protected override void DoDispose()
		{
			m_request = null;
			base.DoDispose();
		}
	}
}
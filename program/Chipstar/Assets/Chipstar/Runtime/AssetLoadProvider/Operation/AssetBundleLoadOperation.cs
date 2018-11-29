﻿using System;
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
		private AssetBundleRequest	m_request		= null;
		private IDisposable         m_unloadDispose = null;
		//====================================
		//	プロパティ
		//====================================
		public override bool	keepWaiting { get { return !m_request.isDone; } }
		public override T		Content		{ get { return m_request.asset as T; } }
		//====================================
		//	関数
		//====================================

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public AssetBundleLoadOperation( AssetBundleRequest request, IDisposable unloadDispose )
		{
			m_request		= request;
			m_unloadDispose = unloadDispose;
		}

		/// <summary>
		/// 破棄処理
		/// </summary>
		protected override void DoDispose()
		{
			if( m_unloadDispose != null)
			{
				m_unloadDispose.Dispose();
			}
			m_unloadDispose = null;
			m_request       = null;
		}
	}
}
﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Chipstar.AssetLoad
{
	/// <summary>
	/// Resources.LoadAsyc用
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public sealed class ResourcesLoadOperation<T> 
		: LoadOperation<T> 
		where T : UnityEngine.Object
	{
		//===============================
		//	変数
		//===============================
		private ResourceRequest m_request = null;

		//===============================
		//	プロパティ
		//===============================
		public override bool	keepWaiting { get { return !m_request.isDone; } }
		public override T		Content		{ get { return m_request.asset as T; } }

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
	}
}
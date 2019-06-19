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
		private string m_key = string.Empty;
		private ResourceRequest m_request = null;

		//===============================
		//	関数
		//===============================
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ResourcesLoadOperation( string key )
		{
			m_key = key;
		}

		protected override void DoRun()
		{
			m_request = Resources.LoadAsync<T>(m_key);
		}

		protected override ResultCode DoError(Exception e)
		{
			return ChipstarResult.ClientError($"Resources Assets Load Error :: {m_key}", e);
		}

		/// <summary>
		/// 破棄
		/// </summary>
		protected override void DoDispose()
		{
			m_request = null;
			base.DoDispose();
		}

		protected override void DoPostUpdate()
		{
		}

		protected override void DoComplete()
		{
			base.DoComplete();
			// Resourcesは参照計算とかいらないので自動でオペレータを破棄する
			this.DisposeIfNotNull();
		}

		protected override float GetProgress()
		{
			return m_request?.progress ?? 1;
		}
		protected override bool GetComplete()
		{
			return m_request.isDone;
		}
		protected override T GetContent()
		{
			return m_request.asset as T;
		}

		public override string ToString()
		{
			return "[ResourcesLoadOperation]" + m_key;
		}
	}
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Chipstar.Downloads
{
	public interface IErrorHandler : IDisposable
	{
		bool IsError { get; }
		Action<IReadOnlyList<ResultCode>> OnError { set; }

		void Init();
		void Receive( ResultCode code );
		void Update();
	}
	/// <summary>
	/// 
	/// </summary>
	public sealed class ErrorHandler : IErrorHandler
	{
		//===============================
		//	変数
		//===============================
		private List<ResultCode> m_errorList = new List<ResultCode>();

		//===============================
		//	プロパティ
		//===============================
		public bool IsError { get; private set; }
		public Action<IReadOnlyList<ResultCode>> OnError { private get; set; }

		//===============================
		//	関数
		//===============================
		/// <summary>
		/// 
		/// </summary>
		public void Dispose()
		{
			m_errorList.Clear();
		}

		public void Init()
		{
			IsError = false;
			m_errorList.Clear();
		}

		public void Receive(ResultCode code)
		{
			m_errorList.Add(code);
		}

		public void Update()
		{
			if ( m_errorList.Count <= 0 )
			{
				return;
			}
			var copyLiist = m_errorList.ToArray();
			m_errorList.Clear();
			OnError?.Invoke( copyLiist );
		}
	}
}
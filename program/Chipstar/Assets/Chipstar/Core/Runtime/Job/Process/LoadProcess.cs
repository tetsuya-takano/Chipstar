using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Chipstar.Downloads
{
	public sealed class Empty
	{
		public static readonly Empty Default = new Empty();
	}

	/// <summary>
	/// ロードを処理受付
	/// </summary>
	public interface ILoadProcess : ILoadStatus, 
		IEnumerator
	{

	}
	/// <summary>
	/// 結果を取る処理
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface ILoadProcess<T> : ILoadProcess, ILoadStatus<T>
	{
	}
	/// <summary>
	/// 結果を取るロード処理
	/// </summary>
	public sealed class LoadProcess<T> : ILoadProcess<T>
	{
		//=====================================
		//  変数
		//=====================================
		private ILoadJob<T> m_job = null;
		private Action<T> m_onCompleted = null;

		//=====================================
		//  プロパティ
		//=====================================
		public T Content { get; private set; }
		public bool IsCompleted { get; private set; }
		public bool IsError { get; private set; }

		public float Progress { get { return m_job == null ? 0 : m_job.Progress; } }
		object IEnumerator.Current => null;

		//=====================================
		//  関数
		//=====================================

		public LoadProcess(
			ILoadJob<T> job,
			Action<T> onCompleted,
			Action<ChipstarResultCode> onError = null
		)
		{
			m_onCompleted = onCompleted;
			m_job = job;
			m_job.OnSuccess = () =>
		   {
			   Content = m_job.Content;
			   var act = m_onCompleted;
			   m_onCompleted = null;
			   act?.Invoke( Content );

			   IsCompleted = true;
		   };
			m_job.OnError = code =>
			{
				m_onCompleted = null;
				onError?.Invoke( code );
				IsError = true;
			};
		}
		/// <summary>
		/// 
		/// </summary>
		public void Dispose()
		{
			m_job = null;
			m_onCompleted = null;
			
		}

		bool IEnumerator.MoveNext()
		{
			return !IsCompleted;
		}

		void IEnumerator.Reset() { }
	}

	
}

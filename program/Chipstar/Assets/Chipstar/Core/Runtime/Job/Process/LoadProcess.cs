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
		public bool IsCompleted { get; private set; } = false;
		public bool IsError { get; private set; } = false;

		public float Progress => m_job?.Progress ?? 0;
		object IEnumerator.Current => null;

		public bool IsCanceled => m_job?.IsCanceled ?? false;

		public bool IsDisposed => m_job?.IsDisposed ?? true;
		public bool IsRunning => m_job?.IsRunning ?? false;

		public bool IsFinish => m_job?.IsFinish ?? true;

		//=====================================
		//  関数
		//=====================================

		public LoadProcess(
			ILoadJob<T> job,
			Action<T> onCompleted,
			Action<ResultCode> onError = null
		)
		{
			m_onCompleted = onCompleted;
			m_job = job;
			m_job.OnSuccess = () =>
		   {
			   Content = m_job.Content;
			   ChipstarUtils.OnceInvoke( ref m_onCompleted, Content );
			   IsCompleted = true;
		   };
			m_job.OnError = code =>
			{
				m_onCompleted = null;
				onError?.Invoke(code);
				IsError = true;
			};
		}
		/// <summary>
		/// 
		/// </summary>
		public void Dispose()
		{
			m_onCompleted = null;
			m_job?.Dispose();
			m_job = null;
		}

		bool IEnumerator.MoveNext()
		{
			return !IsCompleted;
		}

		void IEnumerator.Reset() { }
	}

	/// <summary>
	/// プロセスの状態を通知する
	/// </summary>
	public sealed class LoadProcessReporter : ILoadProcess
	{
		//=================================
		//	変数
		//=================================
		private ILoadProcess m_process = null;
		private Action m_onComplete = null;

		//=================================
		//	プロパティ
		//=================================
		public float Progress => m_process?.Progress ?? 1;

		public bool IsCompleted => m_process?.IsCompleted ?? true;

		public bool IsError => m_process?.IsError ?? false;

		public object Current => null;

		public Action OnCompleted { set => m_onComplete = value; }
		public Action<float> OnProgress { set; private get; }

		public bool IsCanceled => m_process?.IsCanceled ?? false;

		public bool IsDisposed => m_process?.IsDisposed ?? true;

		public bool IsRunning => m_process?.IsRunning ?? false;

		public bool IsFinish => m_process?.IsFinish ?? true;

		//=================================
		//	関数
		//=================================

		public LoadProcessReporter( ILoadProcess process)
		{
			m_process = process;
		}

		public void Dispose()
		{
			OnProgress = null;
			OnCompleted = null;
			m_process.DisposeIfNotNull();
			m_process = null;
		}

		/// <summary>
		/// 
		/// </summary>
		public void Report()
		{
			OnProgress?.Invoke( Progress );
			if( IsCompleted && m_onComplete != null )
			{
				ChipstarUtils.OnceInvoke(ref m_onComplete );
			}
		}

		bool IEnumerator.MoveNext()
		{
			return !IsCompleted;
		}

		void IEnumerator.Reset() { }
	}
}

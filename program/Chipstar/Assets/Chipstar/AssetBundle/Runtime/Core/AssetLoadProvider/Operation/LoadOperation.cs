using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
namespace Chipstar.Downloads
{
	/// <summary>
	/// 読み込みタスク
	/// </summary>
	public interface ILoadOperation : ILoadProcess
	{
		Action<ResultCode> OnError { set; }
	}
	public interface ILoadOperater : ILoadOperation
	{
		Action<ILoadOperation> OnStart { set; }
		Action<ILoadOperation> OnStop { set; }

		void Run();
		void Update();
		void Complete();
	}

	/// <summary>
	/// ロード用タスク
	/// </summary>
	public abstract class LoadOperation
		: ILoadOperater
	{
		//===================================
		//	変数
		//===================================
		private Action<ResultCode> m_onError = null;
		private Action<ILoadOperation> m_onStart = null;
		private Action<ILoadOperation> m_onStop = null;

		//===================================
		//	プロパティ
		//===================================
		public bool IsRunning { get; private set; }
		public bool IsCompleted { get; protected set; }
		public Action<ResultCode> OnError
		{
			set => m_onError = value;
			protected get { return m_onError; }
		}

		public Action<ILoadOperation> OnStart { set => m_onStart = value; }
		public Action<ILoadOperation> OnStop { set => m_onStop= value; }

		public float Progress { get; protected set; }
		public bool IsError { get; private set; }
		public bool IsCanceled { get; private set; }
		public bool IsDisposed { get; private set; }
		public bool IsFinish { get; private set; }

		object IEnumerator.Current => null;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public LoadOperation()
		{
		}
		/// <summary>
		/// 破棄処理
		/// </summary>
		void IDisposable.Dispose()
		{
			if (IsDisposed) { return; }
			if ( !IsCompleted )
			{
				Cancel();
			}
			ChipstarLog.Log_Dispose(this);
			DoDispose();

			StopImpl();

			m_onStart = null;
			m_onError = null;
			m_onStop = null;
			IsDisposed = true;
		}
		protected virtual void DoDispose() { }

		protected void StartImpl()
		{
			ChipstarUtils.OnceInvoke(ref m_onStart, this );
		}
		protected void StopImpl()
		{
			ChipstarUtils.OnceInvoke(ref m_onStop, this );
		}

		/// <summary>
		/// 開始
		/// </summary>
		public void Run()
		{
			if (IsDisposed) { return; }
			if (IsCanceled) { return; }
			if (IsError) { return; }
			if (IsCompleted) { return; }

			IsRunning = true;
			StartImpl();
			ChipstarLog.Log_Run(this);
			DoRun();
		}
		protected abstract void DoRun();

		public void Update()
		{
			if( !IsRunning )
			{
				return;
			}
			if (IsCanceled || IsDisposed || IsError || IsCompleted )
			{
				return;
			}
			try
			{
				BeginSample();
				ProcessImpl();
				EndSample();
			}
			catch(Exception e )
			{
				Error(e);
				this.DisposeIfNotNull();
			}
		}
		protected void ProcessImpl()
		{
			DoPreUpdate();
			DoStatusUpdate();
			DoPostUpdate();
		}

		protected virtual void DoPreUpdate() { }
		protected virtual void DoStatusUpdate()
		{
			Progress = GetProgress();
			IsCompleted = GetComplete();
		}

		protected virtual void DoPostUpdate() { }
		void ILoadOperater.Complete()
		{
			StopImpl();
			try
			{
				if (!IsFinish)
				{
					IsFinish = true;
					ChipstarLog.Log_Done(this);
					DoComplete();
				}
			}
			catch (Exception e)
			{
				Error( e );
				throw e;
			}
		}

		private void Error(Exception e)
		{
			StopImpl();

			ChipstarLog.Log_Error(this);
			var result = DoError(e);
			IsError = true;
			ChipstarUtils.OnceInvoke( ref m_onError, result );
		}
		protected virtual ResultCode DoError(Exception e)
		{
			return ChipstarResult.Generic;
		}

		protected abstract void DoComplete();

		protected void Cancel()
		{
			if(IsCanceled) { return; }
			ChipstarLog.Log_Cancel(this);
			DoCancel();
			StopImpl();
			IsCanceled = true;
		}
		protected virtual void DoCancel() { }

		protected abstract float GetProgress();
		protected abstract bool GetComplete();

		bool IEnumerator.MoveNext()
		{
			return !IsCompleted;
		}
		void IEnumerator.Reset() { }

		[Conditional("UNITY_EDITOR")]
		private void BeginSample()
		{
			UnityEngine.Profiling.Profiler.BeginSample(GetSamplingName());
		}
		[Conditional("UNITY_EDITOR")]
		private void EndSample()
		{
			UnityEngine.Profiling.Profiler.EndSample();
		}
		protected virtual string GetSamplingName() { return ToString(); }
	}
}
using System;
using System.Collections;
using UnityEngine;

namespace Chipstar.Downloads
{
	public interface ILoadJob : ILoadStatus, IDisposable, IEnumerator
	{
		Action<ILoadJob> OnStart { set; }
		Action<ILoadJob> OnStop { set; }
		Action OnSuccess { set; }
		Action<ResultCode> OnError { set; }
		bool IsMatch(IAccessLocation location);
		void Run();
		void Update();
		void Done();
		void Error();
		void Cancel();
	}

	public interface ILoadJob<T> : ILoadJob, ILoadStatus<T>
	{
	}


	/// <summary>
	/// DLジョブ
	/// </summary>
	public abstract class LoadJob<THandler, TSource, TData> : ILoadJob<TData>
		where THandler : ILoadJobHandler<TSource, TData>
	{
		//===============================
		//  変数
		//===============================
		private Action m_onSuccess = null;
		private Action<ILoadJob> m_onStart = null;
		private Action<ILoadJob> m_onStop = null;
		private Action<ResultCode> m_onError = null;
		//===============================
		//  プロパティ
		//===============================
		public TData Content { get; private set; }
		public float Progress { get; private set; } = 0f;
		public bool IsRunning { get; private set; } = false;
		public bool IsCompleted { get; private set; } = false;
		public bool IsError { get; private set; } = false;

		public Action<ILoadJob> OnStart { set => m_onStart = value; }
		public Action<ILoadJob> OnStop { set => m_onStop = value; }
		public Action OnSuccess { set => m_onSuccess = value; }
		public Action<ResultCode> OnError { set => m_onError = value; }

		protected IAccessLocation Location { get; private set; }
		protected TSource Source { get; set; }
		protected THandler DLHandler { get; set; }
		public bool IsCanceled { get; private set; } = false;
		public bool IsDisposed { get; private set; } = false;
		object IEnumerator.Current => null;

		public bool IsFinish { get; private set; }

		//===============================
		//  変数
		//===============================

		//===============================
		//  関数
		//===============================

		public LoadJob(IAccessLocation location, THandler handler)
		{
			Location = location;
			DLHandler = handler;
		}

		/// <summary>
		/// 破棄
		/// </summary>
		public void Dispose()
		{
			if (IsDisposed) { return; }
			if (!IsCompleted)
			{
				Cancel();
			}
			ChipstarLog.Log_Dispose(this);
			DoDispose();
			OnSuccess = null;
			OnError = null;
			OnStart = null;
			OnStop = null;
			IsDisposed = true;
		}
		protected virtual void DoDispose()
		{
			DLHandler.Dispose();
			Location.Dispose();
			Source = default;
			DLHandler = default;
			Location = null;
		}

		/// <summary>
		/// 一致判定
		/// </summary>
		public bool IsMatch(IAccessLocation location)
		{
			if( Location == null)
			{
				return false;
			}
			return location.AccessKey == Location.AccessKey;
		}

		/// <summary>
		/// 開始
		/// </summary>
		public void Run()
		{
			if( IsCanceled || IsDisposed )
			{
				return;
			}
			IsRunning = true;
			ChipstarLog.Log_Run(this);
			StartImpl();
			DoRun(Location);
		}
		protected abstract void DoRun(IAccessLocation location);

		private void StartImpl()
		{
			ChipstarUtils.OnceInvoke(ref m_onStart, this);
		}
		/// <summary>
		/// 終了
		/// </summary>
		public void Done()
		{
			IsFinish = true;
			StopImpl();
			ChipstarLog.Log_Done(this);
			DoDone(Source);
		}
		/// <summary>
		/// 完了派生処理
		/// </summary>
		protected virtual void DoDone(TSource source)
		{
			Content = DLHandler.Complete(source);
			ChipstarUtils.OnceInvoke(ref m_onSuccess);
		}

		/// <summary>
		/// エラー処理
		/// </summary>
		public void Error()
		{
			StopImpl();
			ChipstarLog.Log_Error(this);
			var result = DoError(Source);
			ChipstarUtils.OnceInvoke( ref m_onError, result );
		}
		protected virtual ResultCode DoError(TSource source)
		{
			return ChipstarResult.NotImpl;
		}

		private void StopImpl()
		{
			ChipstarUtils.OnceInvoke( ref m_onStop, this );
		}
		/// <summary>
		/// 更新
		/// </summary>
		public void Update()
		{
			ChipstarLog.Log_Update(this);
			//	ジョブのアップデート
			DoUpdate(Source);
			
			//	ジョブステータスの更新
			Progress = GetProgress(Source);
			IsCompleted = GetIsComplete(Source);
			IsError = GetIsError(Source);
		}
		protected virtual void DoUpdate(TSource source) { }

		/// <summary>
		/// キャンセル
		/// </summary>
		public void Cancel()
		{
			if( IsCanceled)
			{
				return;
			}
			StopImpl();

			IsCanceled = true;
			ChipstarLog.Log_Cancel(this);
			DoCancel(Source);
		}
		protected virtual void DoCancel(TSource source ) { }

		protected abstract float GetProgress(TSource source);
		protected abstract bool GetIsComplete(TSource source);
		protected abstract bool GetIsError(TSource source);

		bool IEnumerator.MoveNext()
		{
			return !IsCompleted;
		}

		void IEnumerator.Reset() { }

		public override string ToString()
		{
			return $"{GetType().Name} = {Location?.FullPath ?? string.Empty}";
		}
	}
}
using System;
using UnityEngine;

namespace Chipstar.Downloads
{
	public interface ILoadJob : ILoadStatus, IDisposable
	{
		Action OnSuccess { set; }
		Action<ChipstarResultCode> OnError { set; }
		bool IsMatch(IAccessLocation location);
		void Run();
		void Update();
		void Done();
		void Error();
	}

	public interface ILoadJob<T> : ILoadJob, ILoadStatus<T>
	{
	}


	/// <summary>
	/// DLジョブ
	/// </summary>
	public abstract class LoadJob<THandler, TSource, TData> 
			:	CustomYieldInstruction,
				ILoadJob<TData> where THandler : ILoadJobHandler<TSource, TData>
	{
		//===============================
		//  プロパティ
		//===============================
		public TData Content { get; private set; }
		public float Progress { get; private set; }
		public bool IsCompleted { get; private set; }
		public bool IsError { get; private set; }

		public Action OnSuccess { protected get; set; }
		public Action<ChipstarResultCode> OnError { protected get; set; }

		protected IAccessLocation Location { get; private set; }
		protected TSource Source { get; set; }
		protected THandler DLHandler { get; set; }


		public override bool keepWaiting		{ get { return !IsCompleted; } }
		//===============================
		//  変数
		//===============================

		private bool        m_isDisposed = false;

        //===============================
        //  関数
        //===============================

        public LoadJob( IAccessLocation location, THandler handler )
        {
            Location    = location;
            DLHandler   = handler;
        }

        /// <summary>
        /// 破棄
        /// </summary>
        public void Dispose()
        {
            if (m_isDisposed) { return; }
            DoDispose();
            m_isDisposed = true;
        }
        protected virtual void DoDispose()
        {
            DLHandler.Dispose();
            Location .Dispose();
            Source    = default(TSource);
            DLHandler = default(THandler);
            Location  = null;
            OnSuccess  = null;
			OnError    = null;
        }

		/// <summary>
		/// 一致判定
		/// </summary>
		public bool IsMatch( IAccessLocation location )
		{
			return location.FullPath == Location.FullPath;
		}

        /// <summary>
        /// 開始
        /// </summary>
        public virtual void Run()
        {
			Chipstar.Log_RunJob( Location );
			DoRun( Location );
        }
        protected abstract void DoRun( IAccessLocation location );

        /// <summary>
        /// 終了
        /// </summary>
        public virtual void Done()
        {
			Chipstar.Log_DoneJob( Source, Location );
			DoDone( Source );
        }
		/// <summary>
		/// 完了派生処理
		/// </summary>
        protected virtual void DoDone( TSource source )
        {
            Content = DLHandler.Complete(source);
			var onLoaded = OnSuccess;
			OnSuccess = null;
			onLoaded?.Invoke();
        }

		/// <summary>
		/// エラー処理
		/// </summary>
		public virtual void Error()
		{
			var result = DoError( Source );
			var onError = OnError;
			OnError = null;
			onError?.Invoke(result);
		}
		protected virtual ChipstarResultCode DoError(TSource source)
		{
			return ChipstarResultCode.Generic;
		}

        /// <summary>
        /// 更新
        /// </summary>
        public virtual void Update()
        {
			Chipstar.Log_UpdateJob( Source );
            DoUpdate( Source );
			Progress    = DoGetProgress	( Source );
			IsCompleted = DoIsComplete	( Source );
			IsError		= DoIsError		( Source );
        }

        protected virtual  void		DoUpdate		( TSource source ) { }
		protected abstract float	DoGetProgress	( TSource source );
		protected abstract bool		DoIsComplete	( TSource source );
		protected abstract bool		DoIsError		( TSource source );
	}
}
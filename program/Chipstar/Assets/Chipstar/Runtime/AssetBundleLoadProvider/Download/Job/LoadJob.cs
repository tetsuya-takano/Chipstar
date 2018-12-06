using System;
using UnityEngine;

namespace Chipstar.Downloads
{
    public interface ILoadJob : ILoadTask, IDisposable
    {
		void Run();
        void Update();
        void Done();
    }

    public interface ILoadJob<T> : ILoadJob, ILoadTask<T>
    {
		Action<T> OnLoaded { set; }
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
        public      TData           Content		{ get; private set; }
        public      IAccessLocation Location	{ get; private set; }
        public      float           Progress	{ get; private set; }
        public      bool            IsCompleted	{ get; private set; }
		public		bool			IsError		{ get; private set; }

		public      Action<TData>   OnLoaded	{ protected get; set; }

        protected   TSource  Source				{ get; set; }
        protected   THandler DLHandler			{ get; set; }


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
            OnLoaded  = null;
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
            if (OnLoaded == null)
            {
                return;
            }
            OnLoaded( Content );
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
using System;
using UnityEngine;

namespace Chipstar.Downloads
{
    public interface ILoadJob : ILoadTask, IDisposable
    {
        Action OnLoaded { set; }

        void Run();
        void Update();
        void Done();
    }

    public interface ILoadJob<T> : ILoadJob, ILoadTask<T>
    {
    }


    /// <summary>
    /// DLジョブ
    /// </summary>
    public abstract class LoadJob<THandler, TSource, TData> : ILoadJob<TData>
        where THandler  : ILoadJobHandler<TSource, TData>
    {
        //===============================
        //  プロパティ
        //===============================
        public      IAccessLocation Location       { get; protected set; }
        public      float           Progress       { get; protected set; }
        public      bool            IsCompleted    { get; protected set; }
        public      TData           Content        { get; protected set; }
        public      Action          OnLoaded       { protected get; set; }

        protected   TSource  Source         { get; set; }
        protected   THandler DLHandler      { get; set; }

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
			Debug.Log( "Job Run : "  + Location.AccessPath );
			DoRun( Location );
        }
        protected abstract void DoRun( IAccessLocation location );

        /// <summary>
        /// 終了
        /// </summary>
        public virtual void Done()
        {
            DoDone( Source );
        }

        protected virtual void DoDone( TSource source )
        {
            Content = DLHandler.Complete(source);
            if (OnLoaded == null)
            {
                return;
            }
            OnLoaded( );
        }

        /// <summary>
        /// 更新
        /// </summary>
        public virtual void Update()
        {
            DoUpdate( Source );
        }

        protected abstract void DoUpdate( TSource source );
    }
}
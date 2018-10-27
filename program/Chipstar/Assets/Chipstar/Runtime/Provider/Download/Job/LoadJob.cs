using System;

namespace Chipstar.Downloads
{
    public interface ILoadJob : ILoadTask
    {
        void Run();
        void Update();
        void Done();
    }

    /// <summary>
    /// DLジョブ
    /// </summary>
    public abstract class DLJob<THandler, TSource, TLocation, TData> 
                        : ILoadJob
        where THandler  : IDLHandler<TSource,TData>
        where TLocation : IDLLocation
    {
        //===============================
        //  プロパティ
        //===============================
        public      TLocation       Location       { get; protected set; }
        public      float           Progress       { get; protected set; }
        public      bool            IsCompleted    { get; protected set; }
        public      Action<TData>   OnLoaded       { set { DLHandler.OnLoaded = value; } }

        protected   TSource  Source         { get; set; }
        protected   THandler DLHandler      { get; set; }

        //===============================
        //  変数
        //===============================

        private bool        m_isDisposed = false;

        //===============================
        //  関数
        //===============================

        public DLJob( TLocation location, THandler handler )
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
            Location  = default(TLocation);
        }

        /// <summary>
        /// 開始
        /// </summary>
        public virtual void Run()
        {
            DoRun( Location );
        }
        protected abstract void DoRun(TLocation location);

        /// <summary>
        /// 終了
        /// </summary>
        public virtual void Done()
        {
            DoDone( Source );
        }

        protected virtual void DoDone( TSource source )
        {
            DLHandler.Complete( source );
        }

        /// <summary>
        /// 更新
        /// </summary>
        public virtual void Update()
        {
            DoUpdate( Source, Location );
        }

        protected abstract void DoUpdate(TSource source, TLocation location);
    }
}
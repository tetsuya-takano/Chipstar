using UnityEngine;
using System.Collections;
using System;

namespace Chipstar.Downloads
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class WWWDLJob<TData>
        : LoadJob<WWWDL.WWWHandler<TData>, WWW, TData>
    {
        //=================================================
        //  関数
        //=================================================
        public WWWDLJob( IAccessLocation    location, WWWDL.WWWHandler<TData> handler ) : base( location, handler ) { }
        public WWWDLJob( string             url,      WWWDL.WWWHandler<TData> handler ) : this( new UrlLocation(url), handler) { }

        /// <summary>
        /// 実行開始時
        /// </summary>
        protected override void DoRun( IAccessLocation location )
        {
            Source = new WWW( location.AccessPath );
        }
        /// <summary>
        /// 更新処理
        /// </summary>
        protected override void DoUpdate( WWW source, IAccessLocation location )
        {
            Progress    = source.progress;
            IsCompleted = source.isDone;
        }
    }
}
using UnityEngine;
using System.Collections;
using System;

namespace Chipstar.Downloads
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class WWWDLJob<TData>
        : DLJob<WWWDL.WWWHandler<TData>, WWW, UrlLocation, TData>
    {
        //=================================================
        //  関数
        //=================================================
        public WWWDLJob( UrlLocation    location, WWWDL.WWWHandler<TData> handler ) : base( location, handler ) { }
        public WWWDLJob( string         url,      WWWDL.WWWHandler<TData> handler ) : this( new UrlLocation(url), handler) { }

        /// <summary>
        /// 実行開始時
        /// </summary>
        protected override void DoRun(UrlLocation location)
        {
            Source = new WWW( location.Url );
        }
        /// <summary>
        /// 更新処理
        /// </summary>
        protected override void DoUpdate(WWW source, UrlLocation location)
        {
            Progress    = source.progress;
            IsCompleted = source.isDone;
        }
    }
}
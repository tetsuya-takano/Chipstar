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


        protected override void DoDispose()
        {
            Source.Dispose();
            base.DoDispose();
        }
		/// <summary>
		/// 実行開始時
		/// </summary>
		protected override void DoRun( IAccessLocation location )
        {
            Source = new WWW( location.AccessPath );
        }

		protected override float DoGetProgress( WWW source )
		{
			return source.progress;
		}

		protected override bool DoIsComplete( WWW source )
		{
			return source.isDone;
		}

		protected override bool DoIsError( WWW source )
		{
			return source.error.Length > 0;
		}
	}
}
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
            Source = new WWW( location.FullPath );
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
			return source.error != null && source.error.Length > 0;
		}
	}
}
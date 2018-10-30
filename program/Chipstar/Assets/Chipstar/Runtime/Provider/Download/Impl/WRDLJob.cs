using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.Networking;

namespace Chipstar.Downloads
{
    /// <summary>
    /// UnityWebRequestによる通信ジョブ
    /// </summary>
    public sealed class WRDLJob<TData>
        : LoadJob<WRDL.WRHandler<TData>, UnityWebRequest, TData>
    {
        //================================
        //  変数
        //================================
        /// <summary>
        /// 
        /// </summary>
        public WRDLJob( IAccessLocation location, WRDL.WRHandler<TData> handler ) : base( location, handler )
        {

        }

        protected override void DoDispose()
        {
            Source.Dispose();
            base.DoDispose();
        }

        /// <summary>
        /// 開始時
        /// </summary>
        protected override void DoRun( IAccessLocation location )
        {
            Source = DLHandler.CreateRequest( location );
            Source.SendWebRequest();
        }
        /// <summary>
        /// 更新時
        /// </summary>
        protected override void DoUpdate( UnityWebRequest source )
        {
            IsCompleted = source.isDone;
            Progress    = source.downloadProgress;
        }
    }
}
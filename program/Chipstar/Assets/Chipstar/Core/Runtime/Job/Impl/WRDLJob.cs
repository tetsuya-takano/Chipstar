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
            Source.DisposeIfNotNull();
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
		protected override float DoGetProgress( UnityWebRequest source )
		{
			return source.downloadProgress;
		}

		protected override bool DoIsComplete( UnityWebRequest source )
		{
			return source.isDone;
		}

		protected override bool DoIsError( UnityWebRequest source )
		{
			return source.isNetworkError || source.isHttpError;
		}

		/// <summary>
		/// エラー情報を返す
		/// </summary>
		protected override ResultCode DoError(UnityWebRequest source)
		{
			if( source == null )
			{
				return ChipstarResult.Generic;
			}
			if( source.isNetworkError )
			{
				return ChipstarResult.NetworkError( source.responseCode, source.error);
			}

			if( source.isHttpError)
			{
				return ChipstarResult.HttpError( source.responseCode, source.error );
			}

			return ChipstarResult.NotImpl;
		}
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Chipstar.Downloads
{
    public static partial class WRDL
    {
        //==================================
        //  各通信用ジョブの作成
        //==================================
        public static ILoadJob<string> GetTextFile( IAccessLocation location )
        {
            return new WRDLJob<string>( location, new WRDL.TextDL() );
        }
        public static ILoadJob<byte[]> GetBinaryFile( IAccessLocation location )
        {
            return new WRDLJob<byte[]>( location, new WRDL.BytesDL() );
        }
        public static ILoadJob<AssetBundle> GetAssetBundle( IAccessLocation location )
        {
            return new WRDLJob<AssetBundle>( location, new WRDL.AssetBundleDL() );
        }
		public static ILoadJob<Empty> GetFileDL(IAccessLocation source, IAccessLocation local)
		{
			return new WRDLJob<Empty>( source, new WRDL.FileDL( local ));
		}
		//==================================
		//  各データ取得用ハンドラ定義
		//==================================
		/// <summary>
		/// WebRequestで取得する
		/// </summary>
		public abstract class WRHandler<T>
            : DLHandler<UnityWebRequest, T>
        {
            public abstract UnityWebRequest CreateRequest( IAccessLocation location );
        }

        /// <summary>
        /// テキスト
        /// </summary>
        public sealed class TextDL : WRHandler<string>
        {
            public override UnityWebRequest CreateRequest( IAccessLocation location )
            {
                return UnityWebRequest.Get( location.FullPath );
            }

            protected override string DoComplete( UnityWebRequest source )
            {
                return source.downloadHandler.text;
            }
        }
        public sealed class BytesDL : WRHandler<byte[]>
        {
            public override UnityWebRequest CreateRequest( IAccessLocation location )
            {
                return UnityWebRequest.Get( location.FullPath );
            }

            protected override byte[] DoComplete( UnityWebRequest source )
            {
                return source.downloadHandler.data;
            }
        }
        public sealed class AssetBundleDL : WRHandler<AssetBundle>
        {
            public override UnityWebRequest CreateRequest( IAccessLocation location )
            {
#if UNITY_2018_1_OR_NEWER
				return UnityWebRequestAssetBundle.GetAssetBundle( location.FullPath );
#else
				return UnityWebRequest.GetAssetBundle( location.FullPath );
#endif
            }

            protected override AssetBundle DoComplete( UnityWebRequest source )
            {
                return DownloadHandlerAssetBundle.GetContent( source );
            }
        }

		public sealed class FileDL : WRHandler<Empty>
		{
			private IAccessLocation m_local;

			public FileDL(IAccessLocation local)
			{
				m_local = local;
			}

			public override UnityWebRequest CreateRequest(IAccessLocation location)
			{
				var req = UnityWebRequest.Get( location.FullPath );
				var handler = new DownloadHandlerFile( m_local.FullPath );
				handler.removeFileOnAbort = true;
				req.downloadHandler = handler;
				return req;
			}

			protected override Empty DoComplete(UnityWebRequest source)
			{
				return Empty.Default;
			}
		}
	}
}

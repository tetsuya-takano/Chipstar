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
        public static ILoadJob<string> GetString( IAccessLocation location )
        {
            return new WRDLJob<string>( location, new WRDL.TextDL() );
        }
        public static ILoadJob<byte[]> GetBytes( IAccessLocation location )
        {
            return new WRDLJob<byte[]>( location, new WRDL.BytesDL() );
        }
        public static ILoadJob<AssetBundle> GetAssetBundle( IAccessLocation location )
        {
            return new WRDLJob<AssetBundle>( location, new WRDL.AssetBundleDL() );
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
                return UnityWebRequest.Get( location.AccessPath );
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
                return UnityWebRequest.Get( location.AccessPath );
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
                return UnityWebRequest.GetAssetBundle( location.AccessPath );
            }

            protected override AssetBundle DoComplete( UnityWebRequest source )
            {
                return DownloadHandlerAssetBundle.GetContent( source );
            }
        }
    }
}

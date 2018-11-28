using UnityEngine;
using System.Collections;
using System;

namespace Chipstar.Downloads
{
    /// <summary>
    /// WWWでダウンロードするクラス
    /// </summary>
    public static partial class WWWDL
    {
        //==================================
        //  各通信用ジョブの作成
        //==================================
        public static ILoadJob<string>      GetString       ( IAccessLocation location )
        {
            return new WWWDLJob<string>( location, new WWWDL.TextDL() );
        }
        public static ILoadJob<byte[]>      GetBytes        ( IAccessLocation location )
        {
            return new WWWDLJob<byte[]>( location, new WWWDL.BytesDL() );
        }
        public static ILoadJob<AssetBundle> GetAssetBundle  ( IAccessLocation location )
        {
            return new WWWDLJob<AssetBundle>( location, new WWWDL.AssetBundleDL() );
        }

        //==================================
        //  各データ取得用ハンドラ定義
        //==================================
        /// <summary>
        /// WWWで取る機能
        /// </summary>
        public abstract class WWWHandler<T> : DLHandler<WWW, T> { }

        /// <summary>
        /// テキスト
        /// </summary>
        public sealed class TextDL : WWWHandler<string>
        {
            protected override string DoComplete(WWW source)
            {
                return source.text;
            }
        }
        /// <summary>
        /// 生Bytes
        /// </summary>
        public sealed class BytesDL : WWWHandler<byte[]>
        {
            protected override byte[] DoComplete( WWW source )
            {
                return source.bytes;
            }
        }

        /// <summary>
        /// バンドル
        /// </summary>
        public sealed class AssetBundleDL : WWWHandler<AssetBundle>
        {
            protected override AssetBundle DoComplete(WWW source)
            {
                return source.assetBundle;
            }
        }
    }
}
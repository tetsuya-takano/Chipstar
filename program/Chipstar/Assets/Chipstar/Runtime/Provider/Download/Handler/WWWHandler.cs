﻿using UnityEngine;
using System.Collections;
using System;

namespace Chipstar.Downloads
{
    /// <summary>
    /// WWWでダウンロードするクラス
    /// </summary>
    public static partial class WWWDL
    {
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
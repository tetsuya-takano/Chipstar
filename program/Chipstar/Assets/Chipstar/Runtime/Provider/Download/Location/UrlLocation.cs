using UnityEngine;
using System.Collections;
using System;

namespace Chipstar.Downloads
{
    /// <summary>
    /// URLで場所を知らせる
    /// </summary>
    public sealed class UrlLocation : DLLocation
    {
        //=================================
        //  プロパティ
        //=================================
        public string Url { get; private set; }

        public override string AccessPath { get { return Url; } }

        //=================================
        //  関数
        //=================================

        public UrlLocation(string url)
        {
            Url = url;
        }
    }
}
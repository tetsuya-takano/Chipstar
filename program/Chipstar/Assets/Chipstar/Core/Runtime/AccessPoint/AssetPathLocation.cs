using System;
using UnityEngine;

namespace Chipstar.Downloads
{
    /// <summary>
    /// アセットキーでアクセスするため
    /// </summary>
    public sealed class AssetPathLocation : IAccessLocation
    {
        //===============================
        //  プロパティ
        //===============================
        public string FullPath  { get; private set; }
		public string AccessKey { get; private set; }

		//===============================
		//  関数
		//===============================

		public AssetPathLocation( string key, string path )
		{
			AccessKey= key;
            FullPath = path;
        }

        public void Dispose()
        {
        }
    }
}
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
        public string AccessPath { get; private set; }

        //===============================
        //  関数
        //===============================

        public AssetPathLocation( string key )
        {
            AccessPath = key;
        }

        public void Dispose()
        {
        }
    }
}
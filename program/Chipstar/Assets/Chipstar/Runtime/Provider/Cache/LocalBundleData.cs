using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace Chipstar.Downloads
{
    public interface ILocalBundleData : IDisposable
    {
        bool IsMatchKey     ( string    key     ); // 存在するかどうか
        bool IsMatchVersion ( Hash128   version ); // キャッシュ済みかどうか
    }
    /// <summary>
    /// ローカルに保持してるデータ
    /// </summary>
    public abstract class LocalBundleData: ILocalBundleData
    {
        protected string    Key     { get; private set; }
        protected Hash128   Version { get; private set; }
        /// <summary>
        /// 破棄処理
        /// </summary>
        public void Dispose()
        {
        }

        /// <summary>
        /// キャッシュ済みかどうか
        /// </summary>
        public virtual bool IsMatchVersion( Hash128 version )
        {
            return EqualityComparer<Hash128>.Default.Equals( version, Version );
        }
        /// <summary>
        /// ファイルが存在するかどうか
        /// </summary>
        public virtual bool IsMatchKey( string key )
        {
            return EqualityComparer<string>.Default.Equals(key, Key);
        }
    }
}
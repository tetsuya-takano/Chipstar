using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace Chipstar.Downloads
{
    public interface ILocalBundleData<TKey, TVersion> : IDisposable
    {
        bool IsExist ( TKey     key     ); // 存在するかどうか
        bool IsCached( TVersion version ); // キャッシュ済みかどうか
    }
    /// <summary>
    /// ローカルに保持してるデータ
    /// </summary>
    public abstract class LocalBundleData<TKey, TVersion> 
        : ILocalBundleData<TKey, TVersion>
    {
        protected TKey      Key     { get; private set; }
        protected TVersion  Version { get; private set; }
        /// <summary>
        /// 破棄処理
        /// </summary>
        public void Dispose()
        {
        }

        /// <summary>
        /// キャッシュ済みかどうか
        /// </summary>
        public virtual bool IsCached( TVersion version )
        {
            return EqualityComparer<TVersion>.Default.Equals( version, Version );
        }
        /// <summary>
        /// ファイルが存在するかどうか
        /// </summary>
        public virtual bool IsExist(TKey key)
        {
            return EqualityComparer<TKey>.Default.Equals(key, Key);
        }
    }
}
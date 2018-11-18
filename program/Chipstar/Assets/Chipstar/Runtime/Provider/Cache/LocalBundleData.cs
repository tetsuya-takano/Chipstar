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
        void Apply(Hash128 hash );
    }
    /// <summary>
    /// ローカルに保持してるデータ
    /// </summary>
    [Serializable]
    public class LocalBundleData: ILocalBundleData
    {
        //=================================
        //  SerializeField
        //=================================
        [SerializeField] private string m_key  = null;
        [SerializeField] private string m_hash = null;
        [NonSerialized] private Hash128 m_version;
        //=================================
        //  プロパティ
        //=================================
        protected string            Key
        {
            get { return m_key; }
            set { m_key = value; }
        }
        protected Hash128           Version
        {
            get { return m_version; }
            set
            {
                m_version   = value;
                m_hash      = m_version.ToString();
            }
        }

        //=================================
        //  関数
        //=================================

        /// <summary>
        /// 
        /// </summary>
        public LocalBundleData( string key, Hash128 hash )
        {
            Key         = key;
            Version     = hash;
        }

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

        /// <summary>
        /// バージョンを上書き
        /// </summary>
        public void Apply(Hash128 version)
        {
            Version = version;
        }
    }
}
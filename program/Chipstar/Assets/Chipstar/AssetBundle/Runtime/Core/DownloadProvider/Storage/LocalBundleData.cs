using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace Chipstar.Downloads
{
    public interface ILocalBundleData : IDisposable
    {
		Hash128	Version { get; }

        bool IsMatchKey     ( string			key    ); // 一致データがあるかどうか
        bool IsMatchVersion ( ICachableBundle	bundle ); // キャッシュと一致するかどうか
        void Apply			( ICachableBundle   bundle );
    }
    /// <summary>
    /// ローカルに保持してるデータ
    /// </summary>
    [Serializable]
    public class LocalBundleData: ILocalBundleData, ISerializationCallbackReceiver
    {
        //=================================
        //  SerializeField
        //=================================
        [SerializeField] private string  m_key  = null;
        [SerializeField] private string  m_hash = null;
		[SerializeField] private uint	 m_crc  = 0;
		[NonSerialized]  private Hash128 m_version;
        //=================================
        //  プロパティ
        //=================================
        public string            Key
        {
            get { return m_key; }
            set { m_key = value; }
        }
        public Hash128           Version
        {
            get { return m_version; }
			set { m_version = value; }
		}

		public uint CRC
		{
			get { return m_crc; }
			set { m_crc = value; }
		}

		//=================================
		//  関数
		//=================================

		/// <summary>
		/// 
		/// </summary>
		public LocalBundleData( string key, Hash128 hash, uint crc )
        {
            Key         = key;
            Version     = hash;
			CRC         = crc;
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
        public virtual bool IsMatchVersion( ICachableBundle cache )
        {
			return IsMatchHash( cache.Hash ) && IsMatchCRC( cache.Crc );
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
		public void Apply( ICachableBundle bundle )
		{
            Version = bundle.Hash;
			CRC     = bundle.Crc;
        }

		public override string ToString()
		{
			return string.Format( "{0}[{1}]({2})", Key, Version, CRC );
		}

		private bool IsMatchHash( Hash128 hash	) { return Version == hash; }
		private bool IsMatchCRC	( uint crc		) { return CRC	   == crc; }

		/// <summary>
		/// 書き込み時の挙動
		/// </summary>
		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
			m_hash = m_version.ToString();
		}
		/// <summary>
		/// 取得時の挙動
		/// </summary>
		void ISerializationCallbackReceiver.OnAfterDeserialize()
		{
			m_version = Hash128.Parse( m_hash );
		}
	}
}
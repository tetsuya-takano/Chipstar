using UnityEngine;
using System.Collections;
using System;
using System.Text;

namespace Chipstar.Downloads
{
    /// <summary>
    /// キャッシュデータ
    /// </summary>
    public interface ICacheDatabase : IDisposable
    {
        void Load   ( byte[] data );
        bool HasCache<TRuntimeData>(TRuntimeData bundleData) where TRuntimeData : IRuntimeBundleData<TRuntimeData>;
    }
    public class CacheDatabase<TLocalData> : ICacheDatabase
    {
        //===============================================
        //  class
        //===============================================
        [Serializable]
        protected sealed class Table
        {
            [SerializeField] TLocalData[] m_list;
        }
        //===============================================
        //  変数
        //===============================================
        private Table m_table = null;

        //===============================================
        //  関数
        //===============================================
        /// <summary>
        /// 破棄処理
        /// </summary>
        public void Dispose()
        {

        }
        /// <summary>
        /// 
        /// </summary>
        public void Load(byte[] data)
        {
            m_table = ParseLocalTable( data );
        }

        protected virtual Table ParseLocalTable(byte[] data)
        {
            var json = Encoding.UTF8.GetString( data );
            return JsonUtility.FromJson<Table>( json );
        }

        /// <summary>
        /// キャッシュ保持
        /// </summary>
        public bool HasCache<TRuntimeData>(TRuntimeData bundleData) 
            where TRuntimeData : IRuntimeBundleData<TRuntimeData>
        {
            return false;
        }
    }
}
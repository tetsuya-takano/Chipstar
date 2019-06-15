using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Chipstar.Downloads
{
    /// <summary>
    /// 
    /// </summary>
    public interface ISaveFileTable<TData> : IEnumerable<TData> 
        where TData : ILocalBundleData
    {
        TData Get(string key);
        void Add(ICachableBundle data);
        void Remove(TData data);
        void Clear();
    }
    [Serializable]
    public class SaveFileTable<TData> : ISaveFileTable<TData>,
        IEnumerable<TData>, ISerializationCallbackReceiver
        where TData : ILocalBundleData, new()
    {
        //============================================
        //	SerializeField
        //============================================
        [SerializeField] List<TData> m_list = new List<TData>();

        //============================================
        //	変数
        //============================================
        private Dictionary<string, TData> m_table = new Dictionary<string, TData>();

        //============================================
        //	関数
        //============================================
        public TData Get(string key)
        {
            if (m_table.ContainsKey(key))
            {
                return m_table[key];
            }
            return default;
        }

        /// <summary>
        /// 追加
        /// </summary>
        public void Add(ICachableBundle data)
        {
            TData d = default;
            if (m_table.TryGetValue(data.Path, out d))
            {
                d.Apply(data);
                m_table[data.Path] = d;
                return;
            }
            d = new TData();
            d.Apply(data);
            m_table.Add(data.Path, d);
        }

        /// <summary>
        /// 削除
        /// </summary>
        public void Remove(TData localData)
        {
            if (!m_table.ContainsKey(localData.Path))
            {
                return;
            }
            m_table.Remove(localData.Path);
        }

        /// <summary>
        /// クリア
        /// </summary>
        public void Clear()
        {
            m_table.Clear();
            m_list.Clear();
        }

        /// <summary>
        /// 列挙
        /// </summary>
        IEnumerator<TData> IEnumerable<TData>.GetEnumerator()
        {
            return ((IEnumerable<TData>)m_table.Values).GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<TData>)m_table.Values).GetEnumerator();
        }

        /// <summary>
        /// 読み込む時
        /// </summary>
        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            if (m_list == null)
            {
                m_table = new Dictionary<string, TData>();
                return;
            }
            //	List -> Dictionary
            m_table = m_list.ToDictionary(c => c.Path);
        }

        /// <summary>
        /// 保存するとき
        /// </summary>
        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            if (m_table == null)
            {
                m_list = new List<TData>();
                return;
            }
            //	Dictionary -> List
            m_list = m_table.Values.ToList();
        }
    }
}
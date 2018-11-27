using UnityEngine;
using System.Collections;
using System;
using System.Text;
using System.Collections.Generic;
using System.IO;

namespace Chipstar.Downloads
{
    /// <summary>
    /// キャッシュデータ
    /// </summary>
    public interface ICacheDatabase : IDisposable
    {
		IEnumerator Initialize( string localVersionFile );

		bool HasCache	( ICachableBundle data );
        void SaveVersion( ICachableBundle data );
        void Apply( );
	}
    public class CacheDatabase : ICacheDatabase
    {
        //===============================================
        //  class
        //===============================================
        [Serializable]
        protected sealed class Table
        {
			//============================================
			//	SerializeField
			//============================================
			[SerializeField] List<LocalBundleData> m_list = new List<LocalBundleData>();

			//============================================
			//	SerializeField
			//============================================
			public LocalBundleData Find( string key )
            {
                foreach (var d in m_list)
                {
                    if (d.IsMatchKey( key ))
                    {
                        return d;
                    }
                }
                return null;
            }

            /// <summary>
            /// 追加
            /// </summary>
            internal void Add( string key, Hash128 hash )
            {
                m_list.Add(new LocalBundleData(key, hash));
            }
        }
		//===============================================
		//  変数
		//===============================================
		private IEntryPoint     m_entryPoint	= null;
		private IAccessLocation m_versionFile   = null;
        private Table			m_table			= null;

        //===============================================
        //  関数
        //===============================================
        
		public CacheDatabase( IEntryPoint entryPoint )
		{
			m_entryPoint = entryPoint;
		}

		/// <summary>
        /// 破棄処理
        /// </summary>
        public void Dispose()
        {
			m_table = null;
        }

		/// <summary>
		/// 初期化
		/// </summary>
		public IEnumerator Initialize( string versionFile )
		{
			//	TODO : 仮処理
			m_versionFile	= m_entryPoint.ToLocation( versionFile );
			var path		= m_versionFile.AccessPath;
			var isExist = File.Exists( path );
			if( isExist )
			{
				var bytes = File.ReadAllBytes( path );
				Load( bytes );
				yield break;
			}
			//	なければ空データ
			m_table = new Table();
		}

		/// <summary>
		/// 
		/// </summary>
		protected void Load(byte[] data)
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
        public bool HasCache( ICachableBundle bundleData ) 
        {
            var data = m_table.Find( bundleData.Name );
            if ( data == null )
            {
                return false;
            }


            return data.IsMatchVersion( bundleData.Hash );
        }

        /// <summary>
        /// バージョンの保存
        /// </summary>
        public virtual void SaveVersion( ICachableBundle data )
        {
            //  キャッシュテーブルにあるかどうか
            var cache = m_table.Find(data.Name);
            if (cache == null)
            {
                //  なければ追加書き込み
                m_table.Add(data.Name, data.Hash);
                return;
            }
            //  あったらバージョン情報を上書き
            cache.Apply( data.Hash );
        }

        /// <summary>
        /// 保存
        /// </summary>
        public virtual void Apply( )
        {
			var path = m_versionFile.AccessPath;
			var json = JsonUtility.ToJson( m_table, true );
			if( !Directory.Exists( path ))
			{
				Directory.CreateDirectory( path );
			}
			File.WriteAllText( path, json );
        }
	}
}
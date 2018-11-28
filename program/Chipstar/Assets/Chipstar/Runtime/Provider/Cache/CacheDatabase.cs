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
		IEnumerator		Initialize( );
		IAccessLocation ToLocation( string fileName );

		bool HasCache	( ICachableBundle data );
        void SaveVersion( ICachableBundle data, byte[] content );
        void Apply( );
	}
    public class CacheDatabase : ICacheDatabase
    {
        //===============================================
        //  class
        //===============================================
        [Serializable]
        protected sealed class Table : IEnumerable<LocalBundleData>
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

			public IEnumerator<LocalBundleData> GetEnumerator()
			{
				return ( (IEnumerable<LocalBundleData>)m_list ).GetEnumerator();
			}

			/// <summary>
			/// 追加
			/// </summary>
			internal void Add( string key, Hash128 hash )
            {
                m_list.Add(new LocalBundleData(key, hash));
            }

			IEnumerator IEnumerable.GetEnumerator()
			{
				return ( (IEnumerable<LocalBundleData>)m_list ).GetEnumerator();
			}
		}
		//===============================================
		//  変数
		//===============================================
		private string          m_fileName      = null;
		private IEntryPoint     m_entryPoint	= null;
		private IAccessLocation m_versionFile   = null;
        private Table			m_table			= null;

        //===============================================
        //  関数
        //===============================================
        
		public CacheDatabase( IEntryPoint entryPoint, string cacheDbName )
		{
			m_entryPoint = entryPoint;
			m_fileName   = cacheDbName;
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
		public IEnumerator Initialize( )
		{
			//	TODO : 仮処理
			m_versionFile	= m_entryPoint.ToLocation( m_fileName );
			var path		= m_versionFile.AccessPath;
			var isExist = File.Exists( path );
			if( !isExist )
			{
				//	なければ空データ
				m_table = new Table();
			}
			else
			{
				var bytes = File.ReadAllBytes( path );
					Load( bytes );
			}
			yield return null;
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
        public virtual void SaveVersion( ICachableBundle data, byte[] content )
        {
			//	ファイルの書き込み
			WriteBundle( data, content );
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
			var path	= m_versionFile.AccessPath;
			var dirPath = Path.GetDirectoryName( path );

			var json = JsonUtility.ToJson( m_table, true );
			if( !Directory.Exists( dirPath ) )
			{
				Directory.CreateDirectory( dirPath );
			}
			File.WriteAllText( path, json );
        }

		/// <summary>
		/// 場所の取得
		/// </summary>
		public IAccessLocation ToLocation( string fileName )
		{
			return m_entryPoint.ToLocation( fileName );
		}

		/// <summary>
		/// アセットバンドル書き込み
		/// </summary>
		private void WriteBundle( ICachableBundle data, byte[] content )
		{
			var location = m_entryPoint.ToLocation( data.Name );
			var dirPath  = Path.GetDirectoryName( location.AccessPath );
			if( !Directory.Exists( dirPath ) )
			{
				Directory.CreateDirectory( dirPath );
			}
			File.WriteAllBytes( location.AccessPath, content );
		}

		public override string ToString()
		{
			var builder = new StringBuilder();

			foreach( var item in m_table )
			{
				builder.AppendLine( item.ToString() );
			}
			return builder.ToString();
		}
	}
}
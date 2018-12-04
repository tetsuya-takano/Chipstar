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
        void Write		( ICachableBundle data, byte[] content );
        void Apply		( );
		void Delete		( ICachableBundle bundle );
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

			/// <summary>
			/// 削除
			/// </summary>
			internal void Remove( LocalBundleData cache )
			{
				m_list.Remove( cache );
			}

			/// <summary>
			/// 列挙
			/// </summary>
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
			Chipstar.Log_InitCacheDB( path );

			var isExist = File.Exists( path );
			if( !isExist )
			{
				//	なければ空データ
				m_table = new Table();
				Chipstar.Log_InitCacheDB_FirstCreate( path );
			}
			else
			{
				var bytes	= File.ReadAllBytes( path );
				m_table		= Load( bytes );
				Chipstar.Log_InitCacheDB_ReadLocalFile( m_table );
			}
			yield return null;
		}

		/// <summary>
		/// 
		/// </summary>
		protected Table Load(byte[] data)
        {
            return ParseLocalTable( data );
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
        /// キャッシュとバージョンの書き込み
        /// </summary>
        public virtual void Write( ICachableBundle data, byte[] content )
        {
			//	ファイルの書き込み
			WriteBundle( data, content );
			SaveVersion( data );
        }

		/// <summary>
		/// 削除
		/// </summary>
		public virtual void Delete( ICachableBundle data )
		{
			DeleteBundle ( data );
			RemoveVersion( data );
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
			Chipstar.Log_ApplyLocalSaveFile( path );
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
			var filePath = location.AccessPath;
			var dirPath  = Path.GetDirectoryName( filePath );
			if( !Directory.Exists( dirPath ) )
			{
				Directory.CreateDirectory( dirPath );
			}
			if( File.Exists( filePath ) )
			{
				File.Delete( filePath );
			}

			Chipstar.Log_WriteLocalBundle( location );
			File.WriteAllBytes( location.AccessPath, content );
		}
		/// <summary>
		/// バージョンの保存
		/// </summary>
		private void SaveVersion( ICachableBundle data )
		{
			Chipstar.Log_SaveLocalVersion( data );
			//  キャッシュテーブルにあるかどうか
			var cache = m_table.Find(data.Name);
			if( cache == null )
			{
				//  なければ追加書き込み
				m_table.Add( data.Name, data.Hash );
				return;
			}
			//  あったらバージョン情報を上書き
			cache.Apply( data.Hash );
		}

		/// <summary>
		/// アセットバンドルの削除
		/// </summary>
		private void DeleteBundle( ICachableBundle data )
		{
			var location = ToLocation( data.Name );
			var path     = location.AccessPath;
			if( !File.Exists( path ) )
			{
				//	存在しないなら削除しない
				return;
			}
			Chipstar.Log_DeleteLocalBundle( data );
			File.Delete( path );
		}
		/// <summary>
		/// 保存バージョンを破棄
		/// </summary>
		private void RemoveVersion( ICachableBundle data )
		{
			Chipstar.Log_RemoveLocalVersion( data );
			var cache = m_table.Find( data.Name );
			if( cache == null )
			{
				//	保存されてないなら消さなくていい
				return;
			}
			m_table.Remove( cache );
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
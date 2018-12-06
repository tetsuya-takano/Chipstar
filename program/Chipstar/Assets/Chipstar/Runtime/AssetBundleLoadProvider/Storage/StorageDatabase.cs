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
    public interface IStorageDatabase : IDisposable
    {
		IEnumerator		Initialize( );
		IAccessLocation ToLocation( string fileName );

		bool HasStorage	( ICachableBundle data );
        void Write		( ICachableBundle data, byte[] content );
        void Apply		( );
		void Delete		( ICachableBundle bundle );

		Hash128 GetVersion( ICachableBundle data );
	}
    public class StorageDatabase : IStorageDatabase
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
			internal void Remove( LocalBundleData localData )
			{
				m_list.Remove( localData );
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
        
		public StorageDatabase( IEntryPoint entryPoint, string storageDbName )
		{
			m_entryPoint = entryPoint;
			m_fileName   = storageDbName;
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
			Chipstar.Log_InitStorageDB( path );

			var isExist = File.Exists( path );
			if( !isExist )
			{
				//	なければ空データ
				m_table = new Table();
				Chipstar.Log_InitStorageDB_FirstCreate( path );
			}
			else
			{
				var bytes	= File.ReadAllBytes( path );
				m_table		= Load( bytes );
				Chipstar.Log_InitStorageDB_ReadLocalFile( m_table );
			}
			yield return null;
		}

		/// <summary>
		/// 取得
		/// </summary>
		public Hash128 GetVersion( ICachableBundle key )
		{
			var data = m_table.Find( key.Name );
			if( data == null)
			{
				return new Hash128();
			}
			return data.Version;
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
        public bool HasStorage( ICachableBundle bundleData ) 
        {
            var data = m_table.Find( bundleData.Name );
            if ( data == null )
            {
                return false;
            }
			var file = m_entryPoint.ToLocation( bundleData.Name );
			if( !File.Exists( file.AccessPath ))
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
			//  ストレージにあるかどうか
			var storageData = m_table.Find(data.Name);
			if( storageData == null )
			{
				//  なければ追加書き込み
				m_table.Add( data.Name, data.Hash );
				return;
			}
			//  あったらバージョン情報を上書き
			storageData.Apply( data.Hash );
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
			var storageData = m_table.Find( data.Name );
			if( storageData == null )
			{
				//	保存されてないなら消さなくていい
				return;
			}
			m_table.Remove( storageData );
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
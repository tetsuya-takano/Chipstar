using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using UnityEngine;
using System.Text;

namespace Chipstar.Downloads
{
    public interface ILoadDatabase<TRuntimeData> : IDisposable
        where TRuntimeData  : IRuntimeBundleData<TRuntimeData>
    {
        IEnumerable<TRuntimeData>            BundleList { get; }
        IEnumerable<AssetData<TRuntimeData>> AssetList  { get; }

		IEnumerator Clear();
		IEnumerator Create(byte[] data);
		AssetData<TRuntimeData> GetAssetData(string path);
		TRuntimeData GetBundleData(string name);
		bool Contains(string path);
	}

    public class LoadDatabase<TTable, TBundle, TAsset, TRuntimeData> 
            :   ILoadDatabase<TRuntimeData>

            where TBundle       : IBundleBuildData
            where TAsset        : IAssetBuildData
            where TTable        : IBuildMapDataTable<TBundle, TAsset>

            where TRuntimeData  : IRuntimeBundleData<TRuntimeData>, new()
    {
		//=========================================
		//  class
		//=========================================

		//=========================================
		//  変数
		//=========================================
		private Encoding                                    m_encoding      = null;
		private string                                      m_dbFileName    = null;
		private Dictionary<string, TRuntimeData>            m_bundleTable	= new Dictionary<string, TRuntimeData             >( StringComparer.OrdinalIgnoreCase ); // バンドル名   → バンドルデータテーブル
        private Dictionary<string, AssetData<TRuntimeData>> m_assetsTable	= new Dictionary<string, AssetData<TRuntimeData>  >( StringComparer.OrdinalIgnoreCase ); // アセットパス → アセットデータテーブル
		private string                                      m_prefix        = string.Empty;
        //=========================================
        //  プロパティ
        //=========================================
        public IEnumerable<TRuntimeData>            BundleList  { get { return m_bundleTable.Values; } }
        public IEnumerable<AssetData<TRuntimeData>> AssetList   { get { return m_assetsTable.Values; } }

        //=========================================
        //  関数
        //=========================================

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public LoadDatabase( string dbFileName, Encoding encoding )
		{
			m_dbFileName = dbFileName;
			m_encoding   = encoding;
		}

        /// <summary>
        /// 破棄処理
        /// </summary>
        public void Dispose()
        {
            foreach (var item in m_bundleTable)
            {
                item.Value.Dispose();
            }
            m_bundleTable.Clear();
            m_assetsTable.Clear();
        }

		/// <summary>
		/// 
		/// </summary>
		public IEnumerator Create( byte[] datas )
        {
			if( datas == null || datas.Length == 0 )
			{
				Chipstar.Log_Database_NotFound( );
			}
            var table = ParseContentData( datas );
			Chipstar.Log_GetBuildMap<TTable, TBundle, TAsset>( table );
			if( table == null)
			{
				yield break;
			}
			m_prefix  = table.Prefix;
			//  アセットの一覧
			foreach( var asset in table.AssetList)
            {
                var d = new AssetData<TRuntimeData>( asset );
                m_assetsTable.Add( asset.Path, d );
            }
			yield return null;

            //  バンドルの一覧
            foreach (var bundle in table.BundleList)
            {
                var runtime = new TRuntimeData();

                runtime.Set( bundle );

                m_bundleTable.Add(bundle.ABName, runtime);
            }
			yield return null;
			//  依存関係とアセットデータを接続
			foreach (var bundle in table.BundleList)
            {
                var runtime      = m_bundleTable[bundle.ABName];
                var dependencies = CreateDependencies( bundle );
                var assets       = CreateAssets      ( bundle );
                foreach (var asset in assets )
                {
                    asset.Connect( runtime );
                }
                runtime.Set( dependencies );
                runtime.Set( assets );
            }
			yield return null;
		}

		/// <summary>
		/// 既存データの破棄
		/// </summary>
		public IEnumerator Clear()
		{
			foreach( var d in m_bundleTable )
			{
				d.Value.Dispose();
			}
			m_bundleTable.Clear();
			yield return null;
			foreach( var d in m_assetsTable )
			{
				d.Value.Dispose();
			}
			m_assetsTable.Clear();

			yield break;
		}

		protected virtual TTable ParseContentData( byte[] data )
        {
			var json = m_encoding.GetString( data );
			Chipstar.Log_Parse_Result( json );
            return JsonUtility.FromJson<TTable>( json );
        }
        /// <summary>
        /// 依存関係データ作成
        /// </summary>
        private TRuntimeData[] CreateDependencies( TBundle bundle )
        {
            var dependencies = bundle.Dependencies;
            var list = new TRuntimeData[dependencies.Length];
            for (var i = 0; i < dependencies.Length; i++)
            {
                var name = dependencies[i];
                list[i] = m_bundleTable[ name ];
            }
            return list;
        }

        /// <summary>
        /// 含有アセットデータ作成
        /// </summary>
        private AssetData<TRuntimeData>[] CreateAssets(TBundle bundle)
        {
            var assets = bundle.Assets;
            var list = new AssetData<TRuntimeData>[assets.Length];
            for (int i = 0; i < assets.Length; i++)
            {
                var p   = assets[ i ];
                list[i] = m_assetsTable[p];
            }
            return list;
        }

        /// <summary>
        /// 取得
        /// </summary>
        public AssetData<TRuntimeData> GetAssetData( string path )
        {
			if( !m_assetsTable.ContainsKey( path ) )
			{
				return null;
			}
			return m_assetsTable[ path ];
        }

		/// <summary>
		/// バンドルファイル情報取得
		/// </summary>
		public TRuntimeData GetBundleData( string name )
		{
			if( !m_bundleTable.ContainsKey( name ) )
			{
				return default( TRuntimeData );
			}
			return m_bundleTable[ name ];
		}

		/// <summary>
		/// 所持判定
		/// </summary>
		public bool Contains( string path )
		{
			return m_assetsTable.ContainsKey( path );
		}

		/// <summary>
		/// ログ
		/// </summary>
		public override string ToString()
		{
			var builder = new StringBuilder();

			foreach( var b in m_bundleTable.Values )
			{
				builder.AppendLine( b.ToString());
			}

			return builder.ToString();
		}
	}
}
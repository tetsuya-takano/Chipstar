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

		void Clear();
		void Create(byte[] data);
		AssetData<TRuntimeData> GetAssetData(string path);
		TRuntimeData GetBundleData(string name);
		bool Contains(string path);
	}

    public class LoadDatabase<TTable, TBundle, TAsset, TRuntimeData> 
            :   ILoadDatabase<TRuntimeData>

            where TBundle       : IBundleBuildData
            where TAsset        : IAssetBuildData
            where TTable        : IBuildMapDataTable<TBundle, TAsset>, new()

            where TRuntimeData  : IRuntimeBundleData<TRuntimeData>, new()
    {
		//=========================================
		//  class
		//=========================================

		//=========================================
		//  変数
		//=========================================
		private IDatabaseParser<TTable> m_parser = null;
		private Dictionary<string, TRuntimeData> m_bundleTable = new Dictionary<string, TRuntimeData>(StringComparer.OrdinalIgnoreCase); // バンドル名   → バンドルデータテーブル
		private Dictionary<string, AssetData<TRuntimeData>> m_assetsTable	= new Dictionary<string, AssetData<TRuntimeData>  >( StringComparer.OrdinalIgnoreCase ); // アセットパス → アセットデータテーブル
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
		public LoadDatabase( IDatabaseParser<TTable> parser )
		{
			m_parser = parser;
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
		public void Create( byte[] datas )
        {
			if( datas == null || datas.Length == 0 )
			{
				ChipstarLog.Log_Database_NotFound( );
				return;
			}
            var table = m_parser.Parse( datas );
			ChipstarLog.Log_GetBuildMap<TTable, TBundle, TAsset>( table );
			if( table == null)
			{
				return;
			}
			//  アセットの一覧
			foreach( var asset in table.AssetList)
			{
				var d = new AssetData<TRuntimeData>(asset);
				m_assetsTable.Add(asset.Path, d);
			}
			//  バンドルの一覧
			foreach (var bundle in table.BundleList)
			{
				var runtime = new TRuntimeData();

				runtime.Set(bundle);

				m_bundleTable.Add(bundle.ABName, runtime);
			}
			//  依存関係とアセットデータを接続
			foreach (var bundle in table.BundleList)
			{
				var runtime = m_bundleTable[bundle.ABName];
				var dependencies = CreateDependencies(bundle);
				var assets = CreateAssets(bundle);
				foreach (var asset in assets)
				{
					asset.Connect(runtime);
				}
				runtime.Set(dependencies);
				runtime.Set(assets);
			}
		}

		/// <summary>
		/// 既存データの破棄
		/// </summary>
		public void Clear()
		{
			foreach( var d in m_bundleTable )
			{
				d.Value.Dispose();
			}
			m_bundleTable.Clear();
			foreach( var d in m_assetsTable )
			{
				d.Value.Dispose();
			}
			m_assetsTable.Clear();
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
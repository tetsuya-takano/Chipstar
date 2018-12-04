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

        IEnumerator             Initialize			( byte[] data );
		IAccessLocation			ToBuildMapLocation	( );
		AssetData<TRuntimeData> GetAssetData		( string path );
		bool					Contains			( string path );
		IAccessLocation			ToBundleLocation	( TRuntimeData data );
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
		private string                                      m_dbFileName    = null;
		private IEntryPoint                                 m_entryPoint    = null;
        private Dictionary<string, TRuntimeData>            m_bundleTable	= new Dictionary<string, TRuntimeData             >( StringComparer.OrdinalIgnoreCase ); // バンドル名   → バンドルデータテーブル
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
		public LoadDatabase( IEntryPoint entryPoint, string dbFileName )
		{
			m_entryPoint = entryPoint;
			m_dbFileName = dbFileName;
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
			m_entryPoint = null;
        }
        /// <summary>
        /// 
        /// </summary>
        public IEnumerator Initialize( byte[] datas )
        {
            var table = ParseContentData( datas );
			Chipstar.Log_GetBuildMap<TTable, TBundle, TAsset>( table );
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

        protected virtual TTable ParseContentData( byte[] data )
        {
            var json = Encoding.UTF8.GetString( data );
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
                list[i] = m_bundleTable[name];
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
            return m_assetsTable[ path ];
        }
		/// <summary>
		/// 所持判定
		/// </summary>
		public bool Contains( string path )
		{
			return m_assetsTable.ContainsKey( path );
		}

		/// <summary>
		/// コンテンツマニフェストの場所を取得
		/// </summary>
		public IAccessLocation ToBuildMapLocation( )
		{
			return m_entryPoint.ToLocation( m_dbFileName );
		}
		/// <summary>
		/// アセットバンドルの場所を取得
		/// </summary>
		public IAccessLocation ToBundleLocation( TRuntimeData data )
		{
			return m_entryPoint.ToLocation( data.Name );
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
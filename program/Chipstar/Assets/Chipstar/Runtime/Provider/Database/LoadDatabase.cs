using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using UnityEngine;

namespace Chipstar.Downloads
{
    public interface ILoadDatabase<TRuntimeData> : IDisposable
        where TRuntimeData  : IRuntimeBundleData<TRuntimeData>
    {
        IEnumerable<TRuntimeData>            BundleList { get; }
        IEnumerable<AssetData<TRuntimeData>> AssetList  { get; }

        void                    Initialize  ( string json );
        AssetData<TRuntimeData> Find        ( string path );
        IDisposable             AddReference( TRuntimeData data );
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
        private Dictionary<string, TRuntimeData>               m_bundleTable          = new Dictionary<string, TRuntimeData             >( StringComparer.OrdinalIgnoreCase ); // バンドル名   → バンドルデータテーブル
        private Dictionary<string, AssetData<TRuntimeData>>    m_assetsTable          = new Dictionary<string, AssetData<TRuntimeData>  >( StringComparer.OrdinalIgnoreCase ); // アセットパス → アセットデータテーブル

        //=========================================
        //  プロパティ
        //=========================================
        public IEnumerable<TRuntimeData>            BundleList  { get { return m_bundleTable.Values; } }
        public IEnumerable<AssetData<TRuntimeData>> AssetList   { get { return m_assetsTable.Values; } }

        //=========================================
        //  関数
        //=========================================

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
        public void Initialize( string json )
        {
            if (string.IsNullOrEmpty(json))
            {
                Debug.LogError( "LoadError" );
                return;
            }
            var table = JsonUtility.FromJson<TTable>( json );
            //  アセットの一覧
            foreach (var asset in table.AssetList)
            {
                var d = new AssetData<TRuntimeData>(asset.Path, asset.Guid);
                m_assetsTable.Add( asset.Path, d );
            }

            //  バンドルの一覧
            foreach (var bundle in table.BundleList)
            {
                var runtime = new TRuntimeData();

                runtime.Set(bundle.ABName, bundle.Hash);

                m_bundleTable.Add(bundle.ABName, runtime);
            }

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
        public AssetData<TRuntimeData> Find( string path )
        {
            return m_assetsTable[ path ];
        }

        /// <summary>
        /// 参照カウンタの追加
        /// </summary>
        public IDisposable AddReference( TRuntimeData data )
        {
            return new RefCalclater( data );
        }
    }
}
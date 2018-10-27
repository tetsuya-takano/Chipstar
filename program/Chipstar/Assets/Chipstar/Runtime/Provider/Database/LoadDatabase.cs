using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Chipstar.Downloads
{
    public interface ILoadDatabase<TContentTable, TBundle, TAsset, TRuntimeBundle>
        where TBundle       : IBundleBuildData
        where TAsset        : IAssetBuildData
        where TContentTable : IBuildMapDataTable<TBundle, TAsset>
        where TRuntimeBundle: IRuntimeBundleData<TRuntimeBundle>
    {
    }

    public class LoadDatabase<TContentTable, TBundle, TAsset, TRuntimeData> 
            :   ILoadDatabase<TContentTable, TBundle, TAsset, TRuntimeData>

            where TBundle       : IBundleBuildData
            where TAsset        : IAssetBuildData
            where TContentTable : IBuildMapDataTable<TBundle, TAsset>

            where TRuntimeData  : IRuntimeBundleData<TRuntimeData>, new()
    {
        //=========================================
        //  変数
        //=========================================
        private Dictionary<string, TRuntimeData>               m_bundleTable          = new Dictionary<string, TRuntimeData             >( StringComparer.OrdinalIgnoreCase ); // バンドル名   → バンドルデータテーブル
        private Dictionary<string, AssetData<TRuntimeData>>    m_assetsTable          = new Dictionary<string, AssetData<TRuntimeData>  >( StringComparer.OrdinalIgnoreCase ); // アセットパス → アセットデータテーブル

        //=========================================
        //  関数
        //=========================================

        /// <summary>
        /// 
        /// </summary>
        public void Initialize(TContentTable table)
        {
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
        public TRuntimeData Find( string path )
        {
            return m_assetsTable[ path ].BundleData;
        }
    }
}
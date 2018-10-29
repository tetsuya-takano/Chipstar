using Chipstar.Downloads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Chipstar.Downloads
{
    /// <summary>
    /// アセット読み込み処理
    /// </summary>
    public sealed class AssetLoadJob<TRuntimeData> 
        : LoadJob<AssetLoad.AsyncLoad, AssetBundleRequest, UnityEngine.Object>
        where TRuntimeData : IRuntimeBundleData<TRuntimeData>
    {
        //=====================================
        //  関数
        //=====================================
        private TRuntimeData m_bundle = default(TRuntimeData);
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public AssetLoadJob( AssetData<TRuntimeData> assetData )
            : base(new AssetPathLocation( assetData.Path ), new AssetLoad.AsyncLoad())
        {
            m_bundle = assetData.BundleData;
        }

        protected override void DoRun( IAccessLocation location )
        {
            Source = m_bundle.LoadAsync( location.AccessPath );
        }

        protected override void DoUpdate( AssetBundleRequest source )
        {
            Progress    = source.progress;
            IsCompleted = source.isDone;
        }
    }
}

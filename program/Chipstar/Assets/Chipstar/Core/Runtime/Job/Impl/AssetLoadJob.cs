﻿using Chipstar.Downloads;
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
    public sealed class AssetLoadJob<T, TRuntimeData>
        : LoadJob<AssetLoad.AsyncLoad<T>, AssetBundleRequest, T>
        where TRuntimeData : IRuntimeBundleData<TRuntimeData>
        where T : UnityEngine.Object
    {
        //=====================================
        //  関数
        //=====================================
        private TRuntimeData m_bundle = default(TRuntimeData);
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public AssetLoadJob( AssetData<TRuntimeData> assetData )
			: base( new AssetPathLocation( assetData.Guid, assetData.Path ), new AssetLoad.AsyncLoad<T>() )
		{
            m_bundle = assetData.BundleData;
        }

		protected override float GetProgress( AssetBundleRequest source )
		{
			return source.progress;
		}

		protected override bool GetIsComplete( AssetBundleRequest source )
		{
			return source.isDone;
		}

		protected override bool GetIsError( AssetBundleRequest source )
		{
			return source.isDone && source.asset == null;
		}

		protected override void DoRun( IAccessLocation location )
        {
            Source = m_bundle.LoadAsync<T>( location.FullPath );
        }
    }
}

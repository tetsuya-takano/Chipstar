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
    public sealed class AssetLoadJob : LoadJob<AssetLoad.AsyncLoad, AssetBundleRequest, UnityEngine.Object>
    {
        public AssetLoadJob( 
            IAccessLocation     location, 
            AssetLoad.AsyncLoad handler ) 
            : base( location, handler )
        {

        }

        protected override void DoRun( IAccessLocation location )
        {
            
        }

        protected override void DoUpdate( AssetBundleRequest source, IAccessLocation location )
        {
            throw new NotImplementedException();
        }
    }
}

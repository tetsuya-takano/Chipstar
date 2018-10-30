using UnityEngine;
using System.Collections;
using Chipstar.Downloads;
using System;

namespace Chipstar.Example
{
    public class SampleJobCreator<TRuntimeData>
        : JobCreator<TRuntimeData>
        where TRuntimeData : IRuntimeBundleData<TRuntimeData>
    {
        protected override ILoadJob<T> DoCreateAssetLoad<T>( AssetData<TRuntimeData> data )
        {
            return new AssetLoadJob<T, TRuntimeData>( data );
        }

        protected override ILoadJob<AssetBundle> DoCreateBundleLoad( IAccessLocation location )
        {
            return WWWDL.GetAssetBundle( location );
        }

        protected override ILoadJob<byte[]> DoCreateBytesLoad( IAccessLocation location )
        {
            return WWWDL.GetBytes( location );
        }

        protected override ILoadJob<string> DoCreateTextLoad( IAccessLocation location )
        {
            return WWWDL.GetString( location );
        }
    }
}
using UnityEngine;
using System.Collections;

namespace Chipstar.Downloads
{
    /// <summary>
    /// 
    /// </summary>
    public class AssetLoadJob : LoadJob<AssetLoad.AssetLoadHandler, AssetBundleRequest, UrlLocation, UnityEngine.Object>
    {

        public AssetLoadJob( 
            UrlLocation                 location, 
            AssetLoad.AssetLoadHandler  handler
        ) : base(location, handler)
        {
        }
        protected override void DoRun(UrlLocation location)
        {
            
        }

        protected override void DoUpdate(AssetBundleRequest source, UrlLocation location)
        {
            Progress    = source.progress;
            IsCompleted = source.isDone;
        }
    }
}
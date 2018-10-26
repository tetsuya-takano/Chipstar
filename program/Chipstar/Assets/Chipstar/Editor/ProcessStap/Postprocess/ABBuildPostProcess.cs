using System;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Chipstar.Builder
{
    /// <summary>
    /// アセットバンドルビルド後にさせる動作
    /// </summary>
    public interface IABBuildPostProcess<T> where T : IABBuildData
    {
        void OnProcess( IABBuildConfig settings, AssetBundleManifest manifest, IList<T> assetbundleList );
    }

    /// <summary>
    /// 事後処理
    /// </summary>
    public class ABBuildPostProcess<T> : IABBuildPostProcess<T> where T : IABBuildData
    {
        public static ABBuildPostProcess<T> Empty = new ABBuildPostProcess<T>();


        public void OnProcess( 
            IABBuildConfig      settings, 
            AssetBundleManifest manifest, 
            IList<T>            bundleList )
        {
            DoProcess( settings, manifest, bundleList );
        }

        protected virtual void DoProcess( 
            IABBuildConfig      settings,
            AssetBundleManifest manifest,
            IList<T>            bundleList )
        {
            foreach( var d in bundleList )
            {
                Debug.Log( d.ABName );
            }
        }
    }
}
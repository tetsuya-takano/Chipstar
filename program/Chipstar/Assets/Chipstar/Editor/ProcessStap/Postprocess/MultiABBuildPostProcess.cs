using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Chipstar.Builder
{
    /// <summary>
    /// 複数を実行するプロセス
    /// </summary>
    public sealed class MultiABBuildPostProcess<T> : ABBuildPostProcess<T> where T : IABBuildData
    {
        //===============================
        //  変数
        //===============================
        private IEnumerable<IABBuildPostProcess<T>> m_processes = null;

        //===============================
        //  関数
        //===============================

        public MultiABBuildPostProcess( IEnumerable<IABBuildPostProcess<T>> processes )
        {
            m_processes = processes;
        }

        protected override void DoProcess( IABBuildConfig settings, AssetBundleManifest manifest, IList<T> bundleList )
        {
            foreach( var process in m_processes )
            {
                process.OnProcess( settings, manifest, bundleList );
            }
        }
    }

    public static partial class MultiABBuildPostProcessExtensions
    {
        public static IABBuildPostProcess<T> Merge<T>( this IEnumerable<IABBuildPostProcess<T>> self ) where T : IABBuildData
        {
            return new MultiABBuildPostProcess<T>( self );
        }
    }
}
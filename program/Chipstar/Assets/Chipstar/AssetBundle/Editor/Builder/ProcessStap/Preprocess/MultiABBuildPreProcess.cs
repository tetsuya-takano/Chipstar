using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Chipstar.Builder
{
    /// <summary>
    /// 複数を実行するプロセス
    /// </summary>
    public sealed class MultiABBuildPreProcess<T> : ABBuildPreProcess<T> where T : IABBuildData
    {
        //===============================
        //  変数
        //===============================
        private IEnumerable<IABBuildPreProcess<T>> m_processes = null;

        //===============================
        //  関数
        //===============================

        public MultiABBuildPreProcess( IEnumerable<IABBuildPreProcess<T>> processes )
        {
            m_processes = processes;
        }

        protected override void DoProcess( IABBuildConfig config, IList<T> bundleList )
        {
            foreach( var process in m_processes )
            {
				process.OnProcess( config, bundleList );
			}
        }
    }

    public static partial class MultiABBuildPreProcessExtensions
    {
        public static IABBuildPreProcess<T> Merge<T>( this IEnumerable<IABBuildPreProcess<T>> self ) where T : IABBuildData
        {
            return new MultiABBuildPreProcess<T>( self );
        }
    }
}
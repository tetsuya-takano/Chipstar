using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Chipstar.Builder
{
    /// <summary>
    /// 複数を実行するプロセス
    /// </summary>
    public sealed class MultiABBuildPostProcess<T,TResult> 
        : ABBuildPostProcess<T, TResult> 
        where T         : IABBuildData
        where TResult   : IABBuildResult
    {
        //===============================
        //  変数
        //===============================
        private IEnumerable<IABBuildPostProcess<T, TResult>> m_processes = null;

        //===============================
        //  関数
        //===============================

        public MultiABBuildPostProcess(IEnumerable<IABBuildPostProcess<T, TResult>> processes)
        {
            m_processes = processes;
        }

        protected override void DoProcess( IABBuildConfig settings, TResult result, IList<T> bundleList )
        {
            foreach( var process in m_processes )
            {
                process.OnProcess( settings, result, bundleList );
            }
        }
    }

    public static partial class MultiABBuildPostProcessExtensions
    {
        public static IABBuildPostProcess<T,TResult> Merge<T, TResult>( this IEnumerable<IABBuildPostProcess<T, TResult>> self ) 
            where T         : IABBuildData
            where TResult   : IABBuildResult
        {
            return new MultiABBuildPostProcess<T, TResult>( self );
        }
    }
}
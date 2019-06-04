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
    public interface IABBuildPostProcess<T, TResult>
        where T         : IABBuildData
        where TResult   : IABBuildResult
    {
        void OnProcess( IABBuildConfig settings, TResult result, IList<T> assetbundleList );
    }

    /// <summary>
    /// 事後処理
    /// </summary>
    public class ABBuildPostProcess<T, TResult> 
        : IABBuildPostProcess<T,TResult> 
            where T         : IABBuildData
            where TResult   : IABBuildResult
    {
        public static ABBuildPostProcess<T, TResult> Empty = new ABBuildPostProcess<T, TResult>();


        public void OnProcess( 
            IABBuildConfig      settings, 
            TResult             result, 
            IList<T>            bundleList )
        {
            DoProcess( settings, result, bundleList );
        }

        protected virtual void DoProcess( 
            IABBuildConfig      settings,
            TResult             result,
            IList<T>            bundleList )
        {
        }
    }
}
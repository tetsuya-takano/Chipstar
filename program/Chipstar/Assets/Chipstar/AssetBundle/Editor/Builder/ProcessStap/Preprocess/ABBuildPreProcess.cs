using System;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Chipstar.Builder
{
	/// <summary>
	/// アセットバンドルビルド前処理
	/// </summary>
	public interface IABBuildPreProcess<T> where T : IABBuildData
    {
		void OnProcess( IABBuildConfig config, IList<T> assetBundleList );
	}

    public class ABBuildPreProcess<T> : IABBuildPreProcess<T> where T : IABBuildData
    {
        public static readonly ABBuildPreProcess<T> Empty = new ABBuildPreProcess<T>();


        public void OnProcess( IABBuildConfig config, IList<T> bundleList )
        {
            DoProcess( config, bundleList );
        }
		protected virtual void DoProcess( IABBuildConfig config, IList<T> bundleList ) { }
	}
}
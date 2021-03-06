﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Chipstar.Builder
{
    	/// <summary>
	/// アセットバンドルビルド用の設定
	/// </summary>
	public interface IABBuildConfig
	{
		string					RootFolder	{ get; }
		string					OutputPath			{ get; }    //	吐き出し先
		BuildTarget				BuildTarget			{ get; }	//	プラットフォーム
		BuildAssetBundleOptions	Options				{ get; }	//	オプション
	}

    public class ABBuildConfig : IABBuildConfig
    {
        public virtual BuildTarget              BuildTarget			{ get; private set; }
        public virtual BuildAssetBundleOptions  Options				{ get; private set; }
        public virtual string                   OutputPath			{ get; private set; }
		public virtual string					RootFolder	{ get; private set; }

		public ABBuildConfig( 
			string					buildTargetPath,
            string                  outputPath,
            BuildTarget             platform,
            BuildAssetBundleOptions option 
        )
        {
			RootFolder	= buildTargetPath;
            OutputPath			= outputPath;
            Options				= option;
            BuildTarget			= platform;
        }
    }
}

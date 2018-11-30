﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chipstar.Downloads
{
	/// <summary>
	/// UnityWebRequestを使用するタイプ
	/// </summary>
	public class WRJobCreator : JobCreator
	{
		protected override ILoadJob<byte[]> DoCreateBytesLoad( IAccessLocation location )
		{
			return WRDL.GetBinaryFile( location );
		}

		protected override ILoadJob<AssetBundle> DoCreateDownload( IAccessLocation location )
		{
			return WRDL.GetAssetBundle( location );
		}

		protected override ILoadJob<AssetBundle> DoCreateLocalLoad( IAccessLocation location )
		{
			return new LocalFileLoadJob( location );
		}

		protected override ILoadJob<string> DoCreateTextLoad( IAccessLocation location )
		{
			return WRDL.GetTextFile( location );
		}
	}
}
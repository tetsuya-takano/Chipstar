using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Chipstar.Downloads
{
	/// <summary>
	/// WWWクラスを使用するタイプ
	/// </summary>
	public class WWWJobCreator : JobCreator
	{
		protected override ILoadJob<byte[]> DoCreateBytesLoad( IAccessLocation location )
		{
			return WWWDL.GetBinaryFile( location );
		}

		protected override ILoadJob<AssetBundle> DoCreateDownload( IAccessLocation location )
		{
			return WWWDL.GetAssetBundle( location );
		}

		protected override ILoadJob<AssetBundle> DoCreateLocalLoad( IAccessLocation location )
		{
			return new LocalFileLoadJob( location );
		}

		protected override ILoadJob<string> DoCreateTextLoad( IAccessLocation location )
		{
			return WWWDL.GetTextFile( location );
		}
	}
}
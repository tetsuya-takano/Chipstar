using System;
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

		protected override ILoadJob<Empty> DoCreateFileDL(IAccessLocation source, IAccessLocation local)
		{
			return WRDL.GetFileDL(source, local);
		}

		protected override ILoadJob<AssetBundle> DoCreateLocalLoad( IAccessLocation location, Hash128 hash, uint crc )
		{
			return new LocalFileLoadJob( location, crc );
		}

		protected override ILoadJob<string> DoCreateTextLoad( IAccessLocation location )
		{
			return WRDL.GetTextFile( location );
		}
	}
}
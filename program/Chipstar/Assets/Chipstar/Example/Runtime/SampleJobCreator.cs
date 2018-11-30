using UnityEngine;
using System.Collections;
using Chipstar.Downloads;
using System;

namespace Chipstar.Example
{
	public class SampleJobCreator : JobCreator
	{
		protected override ILoadJob<AssetBundle> DoCreateDownload( IAccessLocation location )
		{
            return WWWDL.GetAssetBundle( location );
        }

		protected override ILoadJob<AssetBundle> DoCreateLocalLoad( IAccessLocation location )
		{
			return new LocalFileLoadJob( location );
		}

		protected override ILoadJob<byte[]> DoCreateBytesLoad( IAccessLocation location )
        {
            return WWWDL.GetBinaryFile( location );
        }

        protected override ILoadJob<string> DoCreateTextLoad( IAccessLocation location )
        {
            return WWWDL.GetTextFile( location );
        }
    }
}
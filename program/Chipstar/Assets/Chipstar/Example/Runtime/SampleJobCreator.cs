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
            return WWWDL.GetBytes( location );
        }

        protected override ILoadJob<string> DoCreateTextLoad( IAccessLocation location )
        {
            return WWWDL.GetString( location );
        }
    }
}
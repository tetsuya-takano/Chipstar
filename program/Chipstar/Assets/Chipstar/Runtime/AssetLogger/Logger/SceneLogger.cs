using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Chipstar.Downloads
{

	public class SceneLogger : AssetLogger, ILoadSceneLog
	{
		public SceneLogger( int level ) : base( level )
		{
		}


		protected override void DoLog( string log )
		{
			Debug.LogFormat( "[Scene Logger]" + log );
		}
	}
}
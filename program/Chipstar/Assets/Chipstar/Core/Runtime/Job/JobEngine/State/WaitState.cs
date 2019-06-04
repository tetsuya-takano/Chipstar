using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chipstar.Downloads
{
	public partial class JobEngine
	{
		/// <summary>
		/// 待機
		/// </summary>
		private sealed class WaitState : JobState
		{
			public WaitState(EngineStatus state) : base(state) { }

			public override void Update(ILoadJob job, JobEngine engine)
			{
				if( !engine.MoveNext() )
				{
					return;
				}
				engine.SetState( EngineStatus.Running );
			}
		}
	}
}
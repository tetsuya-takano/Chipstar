using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chipstar.Downloads
{
	public partial class JobEngine
	{
		/// <summary>
		/// エラー
		/// </summary>
		private sealed class ErrorState : JobState
		{
			public ErrorState(EngineStatus state) : base(state) { }

			public override void Begin(ILoadJob job, JobEngine engine)
			{
				job.Error();
				engine.Refresh();
			}
		}
	}
}
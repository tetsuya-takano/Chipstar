using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chipstar.Downloads
{
	public partial class JobEngine
	{
		/// <summary>
		/// 完了時
		/// </summary>
		private sealed class CompleteState : JobState
		{
			public CompleteState(EngineStatus state) : base(state) { }

			public override void Begin(ILoadJob job, JobEngine engine)
			{
				job.Done();
				engine.Refresh();
			}

			public override void Update(ILoadJob job, JobEngine engine)
			{
				engine.SetState(EngineStatus.Wait);
			}
		}
	}
}
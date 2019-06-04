using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chipstar.Downloads
{
	public partial class JobEngine
	{
		/// <summary>
		/// ジョブ状態クラス
		/// </summary>
		private abstract class JobState
		{
			public EngineStatus State { get; }
			public JobState(EngineStatus state) { State = state; }
			/// <summary>
			/// 開始
			/// </summary>
			public virtual void Begin(ILoadJob job, JobEngine engine) { }
			public virtual void Update(ILoadJob job, JobEngine engine) { }
			public virtual void End(ILoadJob job, JobEngine engine) { }
		}
	}
}
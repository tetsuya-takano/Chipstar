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
		private sealed class RunningState : JobState
		{
			public RunningState(EngineStatus state) : base(state) { }

			public override void Begin(ILoadJob job, JobEngine engine)
			{
				job.Run();
			}
			public override void Update(ILoadJob job, JobEngine engine )
			{
				if( job == null )
				{
					engine.SetState(EngineStatus.Wait);
					return;
				}
				//	キャンセル時
				if (job.IsCanceled || job.IsDisposed)
				{
					engine.Refresh();
					engine.SetState(EngineStatus.Wait);
					return;
				}
				job.Update();
				//	エラー
				if( job.IsError )
				{
					engine.SetState( EngineStatus.Error );
					return;
				}
				//	完了
				if( job.IsCompleted )
				{
					engine.SetState( EngineStatus.Complete );
					return;
				}

			}
		}
	}
}
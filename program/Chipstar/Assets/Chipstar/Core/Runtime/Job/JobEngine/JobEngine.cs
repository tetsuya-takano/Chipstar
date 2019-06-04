using System;
using System.Collections.Generic;
using UnityEngine;

namespace Chipstar.Downloads
{
	public interface IJobEngine : IDisposable
	{
		bool HasRequest(IAccessLocation location);
		void Update();
		void Enqueue(ILoadJob request);
		void Cancel();
	}

	//================================
	//  DL用のリクエストを積んで順次処理する
	//================================
	public partial class JobEngine : IJobEngine
	{
		//================================
		//  enum
		//================================
		protected enum EngineStatus
		{
			Wait,
			Running,
			Complete,
			Error
		}

		//================================
		//  変数
		//================================
		private Queue<ILoadJob> m_queue = new Queue<ILoadJob>();
		private ILoadJob m_current = null;
		private readonly Dictionary<EngineStatus, JobState> m_jobStateTable = new Dictionary<EngineStatus, JobState>()
		{
			{ EngineStatus.Wait, new WaitState( EngineStatus.Wait ) },
			{ EngineStatus.Running, new RunningState( EngineStatus.Running ) },
			{ EngineStatus.Complete, new CompleteState( EngineStatus.Complete ) },
			{ EngineStatus.Error, new ErrorState( EngineStatus.Error) },
		};
		private JobState m_state = null;

		//================================
		//  関数
		//================================

		public JobEngine()
		{
			m_state = m_jobStateTable[ EngineStatus.Wait ];
		}

		public void Dispose()
		{
			m_current.DisposeIfNotNull();
			m_current = null;
			m_queue.Clear();
		}

		public void Update()
		{
			//	実行
			m_state.Update(m_current, this);
		}

		/// <summary>
		/// 状態設定
		/// </summary>
		protected void SetState( EngineStatus next )
		{
			var prev = m_state.State;
			if ( prev == next )
			{
				return;
			}
			// end
			m_state.End(m_current, this);

			// next
			m_state = m_jobStateTable[next];
			m_state.Begin(m_current, this);
		}

		protected virtual bool MoveNext()
		{
			if (m_queue.Count <= 0)
			{
				return false;
			}
			m_current = m_queue.Dequeue();
			return true;
		}

		/// <summary>
		/// 追加
		/// </summary>
		public virtual void Enqueue(ILoadJob job)
		{
			Chipstar.Log_AddJob(job);
			m_queue.Enqueue(job);
		}

		/// <summary>
		/// 同一アクセスの検索
		/// </summary>
		public bool HasRequest( IAccessLocation location )
		{
			//	現在処理中のもの
			if( m_current != null )
			{
				if( m_current.IsMatch( location ) )
				{
					return true;
				}
			}

			// 既に積まれているもの
			foreach( var job in m_queue )
			{
				if( job.IsMatch( location ) )
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// キャンセル
		/// </summary>
		public void Cancel()
		{
			if (m_queue.Count > 0)
			{
				m_queue.Clear();
			}
			m_current.DisposeIfNotNull();
			m_current = null;
		}

		private void Refresh()
		{
			//	今のリクエスト消す
			m_current.DisposeIfNotNull();
			m_current = null;
		}
	}
}
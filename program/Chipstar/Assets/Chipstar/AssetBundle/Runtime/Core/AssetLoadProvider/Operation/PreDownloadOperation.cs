using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Chipstar.Downloads
{

	/// <summary>
	/// 
	/// </summary>
	public abstract class PreDownloadOperation : LoadOperation 
	{
		//=======================================
		//	enum
		//=======================================
		private enum State
		{
			Wait,
			PreProcess,
			MainProcess,
		}

		//=======================================
		//	変数
		//=======================================
		private State m_state = State.Wait;
		private string m_path = null;

		private List<ILoadProcess> m_processCache = new List<ILoadProcess>();
		private Queue<Func<string, ILoadProcess>> m_preProcessFuncQueue = new Queue<Func<string, ILoadProcess>>(); // 事前処理
		private ILoadProcess m_waitPreProcess = null;

		//=======================================
		//	prote
		//=======================================
		protected abstract ILoadOperater MainProcess { get; }
		protected string Path { get { return m_path ?? string.Empty; } }

		//=======================================
		//	関数
		//=======================================

		public PreDownloadOperation(string path, Func<string, ILoadProcess>[] preProcess )
		{
			m_path = path;
			foreach (var p in preProcess)
			{
				m_preProcessFuncQueue.Enqueue(p);
			}
		}

		/// <summary>
		/// 開始
		/// </summary>
		protected override void DoRun()
		{
			m_state = State.PreProcess;
			Next();
		}

		protected override void DoDispose()
		{
			m_preProcessFuncQueue.Clear();
			foreach (var p in m_processCache)
			{
				p.DisposeIfNotNull();
			}
			m_processCache.Clear();
			base.DoDispose();
		}

		protected override void DoPreUpdate()
		{
			switch (m_state)
			{
				case State.PreProcess:
					_UpdatePreProcess();
					return;
				case State.MainProcess:
					UpdateMainProcess();
					return;
			}
		}

		private void _UpdatePreProcess()
		{
			// 事前処理を進行する
			if (m_waitPreProcess == null || m_waitPreProcess.IsCompleted)
			{
				// 次の処理を開始する
				Next();
			}
		}

		protected abstract void UpdateMainProcess();

		private void Next()
		{
			// 処理待ち or 完了？
			if (m_preProcessFuncQueue.Count <= 0)
			{
				//	事前処理がすべて完了している
				m_state = State.MainProcess;
				return;
			}
			var func = m_preProcessFuncQueue.Dequeue();
			var p = func?.Invoke(m_path);
			m_waitPreProcess = p;
			m_processCache.Add(p);
		}
	}
}
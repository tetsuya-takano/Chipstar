using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chipstar.Downloads
{
	public sealed class DownloadSceneOperation : PreDownloadOperation, ISceneLoadOperation
	{
		//===========================
		// 変数
		//===========================
		private ISceneLoadOperater m_mainProcess = null;

		//===========================
		// プロパティ
		//===========================
		public Action OnCompleted { set; private get; }

		protected override ILoadOperater MainProcess => m_mainProcess;

		//=======================================
		//	関数
		//=======================================

		public DownloadSceneOperation(string path, Func<string, ILoadProcess>[] preProcess, ISceneLoadOperater main)
			: base(path, preProcess)
		{
			m_mainProcess = main;
		}

		protected override void DoDispose()
		{
			m_mainProcess?.Dispose();
			base.DoDispose();
		}
		/// <summary>
		/// メイン処理
		/// </summary>
		protected override void UpdateMainProcess()
		{
			if (!m_mainProcess.IsRunning)
			{
				//	開始
				m_mainProcess.OnCompleted = OnCompleted;
				m_mainProcess.Run();
			}
			m_mainProcess?.Update();
		}

		protected override bool GetComplete()
		{
			return m_mainProcess?.IsCompleted ?? false;
		}

		protected override float GetProgress()
		{
			return m_mainProcess?.Progress ?? 0;
		}

		protected override void DoComplete()
		{
			m_mainProcess?.Complete();
		}
	}
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Chipstar.Downloads
{

	/// <summary>
	/// DLを同時に行うやつ
	/// </summary>
	public sealed class DownloadAssetOperation<T> 
		: PreDownloadOperation
		, IAssetLoadOperater<T> where T : UnityEngine.Object
	{
		//===========================
		// 変数
		//===========================
		private IAssetLoadOperater<T> m_mainProcess = null;
		//===========================
		// プロパティ
		//===========================

		public Action<T> OnCompleted { set; private get; }
		public T Content { get { return m_mainProcess?.Content; } }
		protected override ILoadOperater MainProcess => m_mainProcess;

		//=======================================
		//	関数
		//=======================================

		public DownloadAssetOperation(string path, Func<string, ILoadProcess>[] preProcess, IAssetLoadOperater<T> main)
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
				m_mainProcess.OnError = OnError;
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
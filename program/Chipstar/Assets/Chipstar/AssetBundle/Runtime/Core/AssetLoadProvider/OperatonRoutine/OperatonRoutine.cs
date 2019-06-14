using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Chipstar.Downloads
{
	/// <summary>
	/// ロード処理を回すやつ
	/// </summary>
	public sealed class OperationRoutine
	{
		//=====================================
		//	変数
		//=====================================
		private List<ILoadOperater> m_runList = new List<ILoadOperater>();
		private List<ILoadOperater> m_completeList = new List<ILoadOperater>();
		private List<ILoadOperater> m_disposedList = new List<ILoadOperater>();

		//=====================================
		//	関数
		//=====================================

		public T Register<T>(T operation) where T : ILoadOperater
		{
			m_runList.Add( operation );

			return operation;
		}
		/// <summary>
		/// 更新
		/// </summary>
		public void Update()
		{
			if (m_completeList.Count > 0)
			{
				foreach (var r in m_completeList)
				{
					r.Complete();
				}
				m_completeList.Clear();
			}
			// Dispose Prev-Completed
			if (m_disposedList.Count > 0)
			{
				foreach (var v in m_disposedList)
				{
					v.DisposeIfNotNull();
				}
				// 可動キューから削除
				m_runList.RemoveAll(c => m_disposedList.Exists(d => d == c));
				m_completeList.RemoveAll(c => m_disposedList.Exists(d => d == c));

				m_disposedList.Clear();
			}

			// 更新処理
			foreach (var r in m_runList)
			{
				if( !r.IsRunning )
				{
					r.Run();
				}
				r.Update();
				if (r.IsCompleted)
				{
					//	完了登録
					m_completeList.Add( r );
				}
				if( r.IsDisposed )
				{
					m_disposedList.Add( r );
				}
			}
		}

		public void Clear()
		{
			m_runList.ForEach(c => c.DisposeIfNotNull());
			m_runList.Clear();
			m_completeList.Clear();
			m_disposedList.Clear();
		}
	}
}
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
		private List<ILoadOperation> m_routineList = new List<ILoadOperation>();
		private List<ILoadOperation> m_destroyList = new List<ILoadOperation>();

		//=====================================
		//	関数
		//=====================================

		public T Register<T>(T operation) where T : ILoadOperation
		{
			m_routineList.Add( operation );

			return operation;
		}
		/// <summary>
		/// 更新
		/// </summary>
		public void Update()
		{
			// Dispose Prev-Completed
			if (m_destroyList.Count > 0)
			{
				foreach (var v in m_destroyList)
				{
					v.DisposeIfNotNull();
				}
				// 可動キューから削除
				m_routineList.RemoveAll(c => m_destroyList.Exists(d => d == c));

				m_destroyList.Clear();
			}

			// 更新処理
			foreach (var r in m_routineList)
			{
				r.Update();
				if (r.IsCompleted)
				{
					//	完了登録
					r.Complete();
					m_destroyList.Add(r);
				}
			}
		}

		public void Clear()
		{
			m_routineList.ForEach(c => c.DisposeIfNotNull());
			m_routineList.Clear();
			m_destroyList.Clear();
		}
	}
}
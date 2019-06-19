using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Chipstar.Downloads
{
	/// <summary>
	/// ロード結果処理を直列にする
	/// </summary>
	public sealed class JoinLoadProcess : ILoadProcess
	{
		//================================
		//  変数
		//================================
		private ILoadProcess m_prev = null;
		private ILoadProcess m_next = null;

		//================================
		//  プロパティ
		//================================
		public bool IsCompleted
		{
			get
			{
				if (!m_prev.IsCompleted)
				{
					return false;
				}
				if (!m_next.IsCompleted)
				{
					return false;
				}
				return true;
			}
		}
		public bool IsError { get { return m_prev.IsError || m_next.IsError; } }
		public float Progress { get { return Mathf.InverseLerp(0, 2, m_prev.Progress + m_next.Progress); } }

		object IEnumerator.Current => null;

		public bool IsCanceled => m_prev.IsCanceled || m_next.IsCanceled;

		public bool IsDisposed => m_prev.IsDisposed && m_next.IsDisposed;

		public bool IsRunning => m_prev.IsRunning || m_next.IsRunning;

		public bool IsFinish => m_prev.IsFinish && m_next.IsFinish;

		//================================
		//  関数
		//================================

		public JoinLoadProcess(ILoadProcess prev, ILoadProcess onNext)
		{
			m_prev = prev;
			m_next = onNext;
		}

		/// <summary>
		/// 破棄処理
		/// </summary>
		public void Dispose()
		{
			m_prev.Dispose();
			m_next.Dispose();
		}

		bool IEnumerator.MoveNext()
		{
			return !IsCompleted;
		}

		void IEnumerator.Reset() { }

		public override string ToString()
		{
			return $"[Join Process]{m_prev?.ToString()} & {m_next?.ToString()}";
		}
	}

	public static partial class ILoadProcessExtensions
	{
		/// <summary>
		/// 直列
		/// </summary>
		public static ILoadProcess ToJoin(this ILoadProcess self, ILoadProcess next)
		{
			return new JoinLoadProcess(self, next);
		}
	}
}

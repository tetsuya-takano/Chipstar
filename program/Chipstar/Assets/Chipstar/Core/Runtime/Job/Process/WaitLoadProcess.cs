using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chipstar.Downloads
{
	/// <summary>
	/// 待機だけ
	/// </summary>
	public sealed class WaitLoadProcess : ILoadProcess
	{
		//===============================
		//	変数
		//===============================
		private Func<bool> m_onWait = null;

		//===============================
		//	プロパティ
		//===============================
		public bool IsCompleted { get { return m_onWait == null ? true : m_onWait(); } }
		public float Progress { get { return IsCompleted ? 1 : 0; } }

		object IEnumerator.Current => null;

		public bool IsError => false;

		//===============================
		//	関数
		//===============================
		public WaitLoadProcess(Func<bool> onWait)
		{
			m_onWait = onWait;
		}

		/// <summary>
		/// 破棄
		/// </summary>
		public void Dispose()
		{
			m_onWait = null;
		}

		bool IEnumerator.MoveNext()
		{
			return !IsCompleted;
		}

		void IEnumerator.Reset() { }
	}
}

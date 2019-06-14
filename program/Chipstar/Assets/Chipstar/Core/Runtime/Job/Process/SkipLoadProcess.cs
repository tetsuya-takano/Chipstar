using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chipstar.Downloads
{
	/// <summary>
	/// ロードをしないけど積む
	/// </summary>
	public sealed class SkipLoadProcess : ILoadProcess
	{
		public static readonly SkipLoadProcess Default = new SkipLoadProcess();
		public bool IsCompleted { get { return true; } }
		public float Progress { get { return 1; } }

		object IEnumerator.Current => null;

		public bool IsError => false;

		public bool IsCanceled => false;

		public bool IsDisposed => true;

		public bool IsRunning => true;

		private SkipLoadProcess() { }
		public void Dispose() { }


		bool IEnumerator.MoveNext()
		{
			return false;
		}

		void IEnumerator.Reset() { }
	}

}

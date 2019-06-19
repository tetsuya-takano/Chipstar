using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Chipstar.Downloads
{
	/// <summary>
	/// オペレータがほしいときだけの諸々
	/// </summary>
	public abstract class MiscLoadOperation : ILoadOperation
	{
		public Action<ResultCode> OnError { set => throw new NotImplementedException(); }
		public virtual float Progress => 1;
		public virtual bool IsRunning => true;

		public virtual bool IsCompleted => true;

		public virtual bool IsError => true;

		public bool IsCanceled => false;

		public bool IsDisposed => true;

		object IEnumerator.Current => null;

		public virtual bool IsFinish => true;

		public void Complete() { }

		public void Dispose() { }
		bool IEnumerator.MoveNext()
		{
			return !IsCompleted;
		}

		void IEnumerator.Reset() { }
		public void Run() { }

		public void Update() { }
	}


	public sealed class ErrorOpertion : MiscLoadOperation
	{
		public static ErrorOpertion Error => new ErrorOpertion();
		public override bool IsError => true;
	}
}
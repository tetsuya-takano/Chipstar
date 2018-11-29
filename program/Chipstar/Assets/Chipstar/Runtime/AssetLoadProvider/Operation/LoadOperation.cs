using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Chipstar.Downloads
{
	public interface ILoadOperation<T> 
		:	IDisposable
			where T : UnityEngine.Object

	{
		T Content { get; }
	}
	/// <summary>
	/// アセットロード用のタスク
	/// とりあえずUnityのコルーチンで通すためのモノ
	/// </summary>
	public abstract class LoadOperation<T> :
		CustomYieldInstruction,
		ILoadOperation<T> where T : UnityEngine.Object
	{
		public abstract T Content { get; }

		public void Dispose() { DoDispose(); }
		protected virtual void DoDispose() { }
	}
}
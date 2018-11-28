using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Chipstar.AssetLoad
{
	public interface ILoadOperation<T> where T : UnityEngine.Object
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
	}
}
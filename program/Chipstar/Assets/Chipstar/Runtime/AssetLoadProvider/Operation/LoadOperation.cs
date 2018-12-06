using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Chipstar.Downloads
{
	/// <summary>
	/// 読み込みタスク
	/// </summary>
	public interface ILoadOperation : IDisposable
	{

	}
	/// <summary>
	/// アセットタスク
	/// </summary>
	public interface IAssetLoadOperation<T> 
		:	ILoadOperation
			where T : UnityEngine.Object

	{
		T Content { get; }
	}
	public interface ISceneLoadOperation
		: ILoadOperation
	{

	}
	/// <summary>
	/// ロード用タスク
	/// </summary>
	public abstract class LoadOperation 
		:	CustomYieldInstruction,
			ILoadOperation
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public LoadOperation()
		{
			Chipstar.Log_StartOperation( this );
		}

		/// <summary>
		/// 破棄処理
		/// </summary>
		public void Dispose()
		{
			DoDispose();
		}
		protected abstract void DoDispose();
	}

	/// <summary>
	/// アセットロード用のタスク
	/// とりあえずUnityのコルーチンで通すためのモノ
	/// </summary>
	public abstract class AssetLoadOperation<T> 
		:	LoadOperation,
			IAssetLoadOperation<T> where T : UnityEngine.Object
	{
		public abstract T Content { get; }
	}
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Chipstar.Downloads
{
	/// <summary>
	/// 読み込みタスク
	/// </summary>
	public interface ILoadOperation : IEnumerator, IDisposable
	{
		bool IsCompleted { get; }
		void Update();
		void Complete();
	}
	/// <summary>
	/// アセットタスク
	/// </summary>
	public interface IAssetLoadOperation<T> 
		:	ILoadOperation
			where T : UnityEngine.Object

	{
		T Content { get; }
		Action<T> OnCompleted { set; }
	}
	public interface ISceneLoadOperation
		: ILoadOperation
	{
		Action OnCompleted { set; }
	}
	/// <summary>
	/// ロード用タスク
	/// </summary>
	public abstract class LoadOperation
		: ILoadOperation
	{
		public abstract bool IsCompleted { get; }
		object IEnumerator.Current => null;
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
		void IDisposable.Dispose()
		{
			DoDispose();
		}
		protected virtual void DoDispose() { }
		void ILoadOperation.Update()
		{
			DoUpdate();
		}
		protected virtual void DoUpdate() { }
		void ILoadOperation.Complete()
		{
			DoComplete();
		}
		protected abstract void DoComplete();

		bool IEnumerator.MoveNext()
		{
			return !IsCompleted;
		}

		void IEnumerator.Reset() { }
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
		public Action<T> OnCompleted { set; private get; }


		protected override void DoComplete()
		{
			var onComplete = OnCompleted;
			OnCompleted = null;
			onComplete?.Invoke( Content );
		}

		protected override void DoDispose()
		{
			OnCompleted = null;
			base.DoDispose();
		}
	}
}
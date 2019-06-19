using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Chipstar.Downloads
{

	/// <summary>
	/// コルーチン化する
	/// </summary>
	public sealed class CoroutineLoadProcess : CustomYieldInstruction, ILoadProcess
	{
		//========================================
		//	変数
		//========================================
		private ILoadProcess m_self = null;

		//========================================
		//	プロパティ
		//========================================
		public override bool keepWaiting { get { return !IsCompleted; } }
		public float Progress => m_self?.Progress ?? 0;
		public bool IsCompleted => m_self?.IsCompleted ?? true;
		public bool IsError => m_self?.IsError ?? false;
		public bool IsRunning => m_self?.IsRunning ?? false;
		public bool IsCanceled => m_self?.IsCanceled ?? false;
		public bool IsDisposed => m_self?.IsDisposed ?? true;

		public bool IsFinish => m_self?.IsFinish ?? true;

		//========================================
		//	関数
		//========================================
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public CoroutineLoadProcess(ILoadProcess result)
		{
			m_self = result;
		}

		public void Dispose()
		{
			m_self = null;
		}

		public override string ToString()
		{
			return "[ Coroutine Process]";
		}
	}
	public static partial class ILoadProcessExtensions
	{

		/// <summary>
		/// 
		/// </summary>
		public static CoroutineLoadProcess ToYieldInstruction(this ILoadProcess self)
		{
			return new CoroutineLoadProcess(self);
		}
	}
}

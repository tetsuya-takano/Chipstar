﻿using System;
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
		public float Progress { get { return m_self != null ? m_self.Progress : 0; } }
		public bool IsCompleted => m_self?.IsCompleted ?? true;
		public bool IsError => m_self?.IsError ?? false;

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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chipstar.Downloads
{
	public interface IRefCountable
	{
		bool IsFree { get; }
		int RefCount { get; }
		void AddRef();
		void ReleaseRef();
		void ClearRef();
	}

	public interface IDeepRefCountable : IRefCountable
	{
		void AddDeepRef();
		void ReleaseDeepRef();
	}
	/// <summary>
	/// 空のアセット参照として使用するクラス
	/// </summary>
	public sealed class EmptyReference : IDisposable
	{
		public static readonly EmptyReference Default = new EmptyReference();
		public void Dispose() { }
	}

	/// <summary>
	/// 参照カウンタの計算クラス
	/// </summary>
	public sealed class RefCalclater : IDisposable
	{
		//======================
		//  変数
		//======================
		private IRefCountable m_data = null;

		private bool m_isDisposed = false;
		//======================
		//  関数
		//======================

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public RefCalclater(IRefCountable data)
		{
			m_data = data;
			m_data?.AddRef();
		}

		/// <summary>
		/// 
		/// </summary>
		public void Dispose()
		{
			if (m_isDisposed)
			{
				return;
			}
			m_data?.ReleaseRef();
			m_data = null;
			m_isDisposed = true;
		}
	}
}

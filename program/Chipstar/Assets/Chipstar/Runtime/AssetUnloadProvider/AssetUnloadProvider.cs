using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chipstar.Downloads
{
	/// <summary>
	/// 破棄管理
	/// </summary>
	public interface IAssetUnloadProvider : IDisposable
	{
		IDisposable AddReference( IRefCountable data );
	}
	/// <summary>
	/// 
	/// </summary>
	public class AssetUnloadProvider : IAssetUnloadProvider
	{
		//========================================
		//	関数
		//========================================

		/// <summary>
		/// 破棄
		/// </summary>
		public void Dispose()
		{

		}

		/// <summary>
		/// 参照の加算
		/// </summary>
		public IDisposable AddReference( IRefCountable data )
		{
			return new RefCalclater( data );
		}
	}
}
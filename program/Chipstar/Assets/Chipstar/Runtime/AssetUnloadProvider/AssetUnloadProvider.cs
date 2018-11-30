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

	}
	/// <summary>
	/// 
	/// </summary>
	public class AssetUnloadProvider : IAssetUnloadProvider
	{
		/// <summary>
		/// 破棄
		/// </summary>
		public void Dispose()
		{
		}
	}
}
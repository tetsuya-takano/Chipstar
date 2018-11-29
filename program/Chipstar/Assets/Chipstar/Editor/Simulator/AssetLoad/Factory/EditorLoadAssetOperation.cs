using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chipstar.Downloads
{
	/// <summary>
	/// 
	/// </summary>
	public sealed class EditorLoadAssetOperation<T> 
		: AssetLoadOperation<T>
		where T : UnityEngine.Object
	{
		//===============================
		//	プロパティ
		//===============================
		public override T		Content		{ get { return null; } }
		public override bool	keepWaiting { get { return false; } }

		//===============================
		//	関数
		//===============================

		/// <summary>
		/// 破棄
		/// </summary>
		protected override void DoDispose()
		{
		}
	}
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chipstar.Downloads
{
	/// <summary>
	/// エディタ用のアセットロード機能
	/// </summary>
	public sealed class AssetLoadSimulator : IAssetLoadProvider
	{
		//=================================
		//	プロパティ
		//=================================
		private IFactoryContainer Container { get; set; }

		//=================================
		//	関数
		//=================================

		public AssetLoadSimulator()
		{
			Container = new FactoryContainer
				(
					new ResourcesLoadFactory()
				);
		}

		/// <summary>
		/// 破棄処理
		/// </summary>
		public void Dispose()
		{

		}
		/// <summary>
		/// アセット
		/// </summary>
		public IAssetLoadOperation<T> LoadAsset<T>( string path ) where T : UnityEngine.Object
		{
			return null;
		}

		/// <summary>
		/// シーン
		/// </summary>
		public ISceneLoadOperation LoadLevel( string path )
		{
			return null;
		}
	}
}
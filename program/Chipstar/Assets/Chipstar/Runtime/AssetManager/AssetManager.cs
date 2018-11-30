using Chipstar.Downloads;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chipstar
{
	/// <summary>
	/// リソース管理機能の統合インターフェイス
	/// </summary>
	public interface IAssetManager : IDisposable
	{
		/// <summary>
		/// 初期化
		/// </summary>
		IEnumerator	Setup();

		/// <summary>
		/// 事前ロード
		/// </summary>
		IEnumerator Preload( string path );

		/// <summary>
		/// アセットの読み込み
		/// </summary>
		IAssetLoadOperation<T>	LoadAsset<T>( string assetPath ) where T : UnityEngine.Object;

		/// <summary>
		/// シーン遷移
		/// </summary>
		ISceneLoadOperation LoadLevel( string scenePath );

		/// <summary>
		/// シーン加算
		/// </summary>
		ISceneLoadOperation LoadLevelAdditive( string scenePath );

		/// <summary>
		/// 更新処理
		/// </summary>
		void DoUpdate();
	}

	/// <summary>
	/// 
	/// </summary>
	public static partial class AssetManager
	{

	}
}
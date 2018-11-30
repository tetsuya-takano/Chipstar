using Chipstar.Downloads;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chipstar
{
	/// <summary>
	/// リソース管理機能の統合クラス
	/// </summary>
	public interface IAssetManager
	{

	}
	/// <summary>
	/// アセット管理クラス
	/// </summary>
	public abstract class AssetManager : IAssetManager
	{
		/// <summary>
		/// 初期化
		/// </summary>
		public abstract IEnumerator	Setup();

		/// <summary>
		/// 事前ロード
		/// </summary>
		public abstract IEnumerator Preload( string path );

		/// <summary>
		/// アセットの読み込み
		/// </summary>
		public abstract IAssetLoadOperation<T>	LoadAsset<T>() where T : UnityEngine.Object;

		/// <summary>
		/// シーン遷移
		/// </summary>
		public abstract ISceneLoadOperation LoadLevel( string scenePath );

		/// <summary>
		/// シーン加算
		/// </summary>
		public abstract ISceneLoadOperation LoadLevelAdditive( string scenePath );
	}
}
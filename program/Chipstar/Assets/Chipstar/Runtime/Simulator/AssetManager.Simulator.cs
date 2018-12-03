#if UNITY_EDITOR
using Chipstar.Downloads;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Chipstar
{
	/// <summary>
	/// シミュレートモード用
	/// </summary>
	public static partial class AssetManager
	{
		/// <summary>
		/// エディタシミュレータモード
		/// </summary>
		public static IAssetManager Simulator()
		{
			return new AssetManagerEditorSimulator();
		}

		/// <summary>
		/// シミュレータモード用クラス
		/// </summary>
		private sealed class AssetManagerEditorSimulator : IAssetManager
		{
			//====================================
			//	プロパティ
			//====================================
			private IAssetLoadProvider AssetProvider { get; set; }

			//====================================
			//	関数
			//====================================

			/// <summary>
			/// コンストラクタ
			/// </summary>
			public AssetManagerEditorSimulator()
			{
				AssetProvider = new AssetLoadSimulator();
			}

			/// <summary>
			/// 破棄処理
			/// </summary>
			public void Dispose()
			{
				AssetProvider.Dispose();
			}
			/// <summary>
			/// 初期化
			/// </summary>
			public IEnumerator Setup()
			{
				//	特にナシ
				yield return null;
			}

			/// <summary>
			/// 事前ロード
			/// </summary>
			public IEnumerator Preload( string path )
			{
				//	不可
				yield return null;
			}
			public IAssetLoadOperation<T> LoadAsset<T>( string assetPath ) where T : UnityEngine.Object
			{
				return AssetProvider.LoadAsset<T>( assetPath );
			}

			/// <summary>
			/// シーン遷移
			/// </summary>
			public ISceneLoadOperation LoadLevel( string scenePath )
			{
				return AssetProvider.LoadLevel( scenePath );
			}

			/// <summary>
			/// シーン加算
			/// </summary>
			public ISceneLoadOperation LoadLevelAdditive( string scenePath )
			{
				return AssetProvider.LoadLevelAdditive( scenePath );
			}

			/// <summary>
			/// 更新処理
			/// </summary>
			public void DoUpdate()
			{
				//	特にナシ
			}

			/// <summary>
			/// 
			/// </summary>
			public void Release( string assetPath )
			{
				//	何もしない
			}

			/// <summary>
			/// 
			/// </summary>
			public IDisposable CreateAssetReference( string path )
			{
				return EmptyReference.Default;
			}
			/// <summary>
			/// 破棄
			/// </summary>
			public IEnumerator Unload( bool isForceUnloadAll )
			{
				if( isForceUnloadAll )
				{
					yield return Resources.UnloadUnusedAssets();
				}
				yield return Resources.UnloadUnusedAssets();
			}
		}
	}
}
#endif
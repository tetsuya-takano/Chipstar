using System;
using System.Collections;
using System.Collections.Generic;
using Chipstar.Downloads;
using UnityEngine;

namespace Chipstar
{
	/// <summary>
	/// Resourcesのみを使用する型
	/// </summary>
	public static partial class AssetManager
	{
		/// <summary>
		/// Resourcesのみ
		/// </summary>
		public static IAssetManager ResourcesOnly()
		{
			return new AssetManagerResourcesOnly();
		}

		/// <summary>
		/// マネージャクラス
		/// </summary>
		private sealed class AssetManagerResourcesOnly : IAssetManager
		{
			//=========================================
			//	プロパティ
			//=========================================
			private IAssetLoadProvider LoadProvider { get; set; }

			//=========================================
			//	関数
			//=========================================

			/// <summary>
			/// コンストラクタ
			/// </summary>
			public AssetManagerResourcesOnly()
			{
				LoadProvider = new AssetLoadProvider(
					new FactoryContainer
					( 
						new BuiltInSceneLoadFactory(),
						new ResourcesLoadFactory() 
					)
				);
			}

			/// <summary>
			/// 破棄
			/// </summary>
			public void Dispose()
			{
				LoadProvider.Dispose();
				LoadProvider = null;
			}

			/// <summary>
			/// 初期化
			/// </summary>
			public IEnumerator Setup()
			{
				yield return null;
			}

			/// <summary>
			/// 事前ロード
			/// </summary>
			public IEnumerator Preload( string path )
			{
				yield return null;
			}
			/// <summary>
			/// アセット取得
			/// </summary>
			public IAssetLoadOperation<T> LoadAsset<T>( string assetPath ) where T : UnityEngine.Object
			{
				return LoadProvider.LoadAsset<T>( assetPath );
			}

			/// <summary>
			/// シーン遷移
			/// </summary>
			public ISceneLoadOperation LoadLevel( string scenePath )
			{
				return LoadProvider.LoadLevel( scenePath );
			}

			/// <summary>
			/// シーン加算
			/// </summary>
			public ISceneLoadOperation LoadLevelAdditive( string scenePath )
			{
				return LoadProvider.LoadLevelAdditive( scenePath );
			}

			/// <summary>
			/// 参照インスタンス作成
			/// </summary>
			public IDisposable CreateAssetReference( string path )
			{
				return EmptyReference.Default;
			}
			
			/// <summary>
			/// 自前参照解放
			/// </summary>
			public void Release( string assetPath )
			{
				//	とりあえず何もしない
			}

			/// <summary>
			/// 
			/// </summary>
			public IEnumerator Unload( bool isForceUnloadAll )
			{
				yield return Resources.UnloadUnusedAssets();
			}

			/// <summary>
			/// 更新処理
			/// </summary>
			public void DoUpdate()
			{
				
			}
		}
	}
}
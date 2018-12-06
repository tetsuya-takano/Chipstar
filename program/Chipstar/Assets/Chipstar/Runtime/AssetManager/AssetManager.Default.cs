using System;
using System.Collections;
using System.Collections.Generic;
using Chipstar.Downloads;
using UnityEngine;

namespace Chipstar
{
	/// <summary>
	/// デフォルトのアセットマネージャインスタンスを返す
	/// </summary>
	public static partial class AssetManager
	{
		/// <summary>
		/// 
		/// </summary>
		public static IAssetManager Default<T>
			( 
				string serverUrl	, string buildInfoFile,
				string storagePath	, string localSaveInfoFile 
			)
			where T : IRuntimeBundleData<T>, new()
		{
			return new AssetManagerRuntimeDefault<T>( 
						serverUrl, 
						buildInfoFile, 
						storagePath, 
						localSaveInfoFile 
				);
		}

		//==================================
		//	class
		//==================================

		/// <summary>
		/// カスタマイズナシの基本設定
		/// </summary>
		private sealed class AssetManagerRuntimeDefault<TRuntimeBundle> 
									:	IAssetManager
			 where TRuntimeBundle	:	IRuntimeBundleData<TRuntimeBundle>, new()
		{
			//======================================
			//	プロパティ
			//======================================
			private IEntryPoint						AccessServer		{ get; set; }
			private IEntryPoint						LocalStorage		{ get; set; }

			private ILoadDatabase<TRuntimeBundle>	LoadDatabase		{ get; set; }
			private IStorageDatabase				StorageDatabase		{ get; set; }

			private IAssetLoadProvider				AssetLoadProvider	{ get; set; }
			private IAssetBundleLoadProvider		DownloadProvider	{ get; set; }
			private IAssetUnloadProvider			UnloadProvider		{ get; set; }
			private IStorageProvider				StorageProvider		{ get; set; }

			//======================================
			//	関数
			//======================================

			/// <summary>
			/// コンストラクタ
			/// </summary>
			public AssetManagerRuntimeDefault( 
				string serverUrl	, string buildInfoFile,
				string storagePath	, string localSaveInfoFile
			)
			{
				//	接続先
				AccessServer = new EntryPoint( serverUrl );
				//	保存先
				LocalStorage = new EntryPoint( storagePath );
				//	コンテンツカタログ
				LoadDatabase = new LoadDatabase<BuildMapDataTable, BundleBuildData, AssetBuildData, TRuntimeBundle>( AccessServer, buildInfoFile );
				//	キャッシュ情報
				StorageDatabase= new StorageDatabase( LocalStorage, localSaveInfoFile );

				//---------------------------------
				//	ダウンロード機能
				//---------------------------------
				DownloadProvider = new AssetBundleLoadProvider<TRuntimeBundle>
					(
						loadDatabase : LoadDatabase,
						storageDatabase: StorageDatabase,
						dlEngine	 : new JobEngine(),
						jobCreator	 : new WRJobCreator()
					);
				//---------------------------------
				//	キャッシュ情報
				//---------------------------------
				StorageProvider = new StorageProvider<TRuntimeBundle>
					(
						assetDatabase  : LoadDatabase,
						storageDatabase: StorageDatabase
					);
				//---------------------------------
				//	アセットロード機能
				//---------------------------------
				var loadFactContainer = new FactoryContainer
				(
					assets : new IAssetLoadFactory[]
					{
						new AssetBundleLoadFactory<TRuntimeBundle>( LoadDatabase ),
						new ResourcesLoadFactory()
					},
					scenes : new ISceneLoadFactory[]
					{
						new BuiltInSceneLoadFactory(),
						new SceneLoadFactory<TRuntimeBundle>( LoadDatabase ),
					}
				);
				AssetLoadProvider = new AssetLoadProvider
					(
						container : loadFactContainer
					);

				//---------------------------------
				//	破棄機能
				//---------------------------------
				UnloadProvider = new AssetUnloadProvider<TRuntimeBundle>
					(
						LoadDatabase
					);
			}

			/// <summary>
			/// 破棄処理
			/// </summary>
			public void Dispose()
			{
				StorageDatabase		.Dispose();
				LoadDatabase		.Dispose();
				DownloadProvider	.Dispose();
				AssetLoadProvider	.Dispose();
				UnloadProvider		.Dispose();

				StorageDatabase       = null;
				LoadDatabase		= null;
				DownloadProvider	= null;
				AssetLoadProvider	= null;
				UnloadProvider      = null;
			}

			/// <summary>
			/// 初期化
			/// </summary>
			public IEnumerator Setup()
			{
				yield return DownloadProvider.InitLoad();
				Chipstar.Log_Dump_Version_Database( StorageProvider.ToString() );
			}

			/// <summary>
			///事前ロード処理 
			/// </summary>
			public IEnumerator Preload( string path )
			{
				yield return DownloadProvider.Load( path ).ToYieldInstruction();
			}

			/// <summary>
			/// アセットアクセス
			/// </summary>
			public IAssetLoadOperation<T> LoadAsset<T>( string assetPath ) where T : UnityEngine.Object
			{
				UnloadProvider.AddRef( assetPath );
				return AssetLoadProvider.LoadAsset<T>( assetPath );
			}

			/// <summary>
			/// シーン遷移
			/// </summary>
			public ISceneLoadOperation LoadLevel( string scenePath )
			{
						UnloadProvider	 .AddRef	( scenePath );
				return	AssetLoadProvider.LoadLevel	( scenePath );
			}

			/// <summary>
			/// シーン加算
			/// </summary>
			public ISceneLoadOperation LoadLevelAdditive( string scenePath )
			{
				return AssetLoadProvider.LoadLevelAdditive( scenePath );
			}

			/// <summary>
			/// 解放
			/// </summary>
			public void Release( string assetPath )
			{
				UnloadProvider.ReleaseRef( assetPath );
			}

			/// <summary>
			/// 参照カウンタオブジェクトを作成
			/// new で加算、Disposeで減算
			/// </summary>
			public IDisposable CreateAssetReference( string path )
			{
				return UnloadProvider.CreateRefCounter( path );
			}

			/// <summary>
			/// 破棄
			/// </summary>
			public IEnumerator Unload( bool isForceUnloadAll )
			{
				if( isForceUnloadAll )
				{
					yield return UnloadProvider.ForceUnloadAll();
				}
				yield return UnloadProvider.UnloadUnusedAssets();
			}

			/// <summary>
			/// 更新
			/// </summary>
			public void DoUpdate()
			{
				DownloadProvider.DoUpdate();
			}
		}
	}
}
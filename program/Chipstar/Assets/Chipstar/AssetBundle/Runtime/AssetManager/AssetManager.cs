using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine.SceneManagement;

namespace Chipstar.Downloads
{
	/// <summary>
	/// アセットバンドルマネージャクラス
	/// </summary>
	public partial class AssetManager<TBundleData> : IAssetManager<TBundleData>
		where TBundleData : IRuntimeBundleData<TBundleData>
	{
		//======================================
		//	プロパティ
		//======================================
		private IAccessPoint AccessServer { get; set; }
		private ILoadDatabase<TBundleData> LoadDatabase { get; set; }
		private ResourcesDatabase ResourcesDatabase { get; set; }
		private IStorageDatabase StorageDatabase { get; set; }

		private IAssetLoadProvider AssetLoadProvider { get; set; }
		private IDownloadProvider DownloadProvider { get; set; }
		private IAssetUnloadProvider UnloadProvider { get; set; }
		private IStorageProvider<TBundleData> StorageProvider { get; set; }
		private IErrorHandler ErrorHandler { get; set; }

		//======================================
		//	関数
		//======================================

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public AssetManager(
			ILoadDatabase<TBundleData> loadDatabase,
			IStorageDatabase storageDatabase,
			IDownloadProvider downloadProvider,
			IStorageProvider<TBundleData> storageProvider,
			IAssetLoadProvider assetProvider,
			IAssetUnloadProvider unloadProvider,
			IErrorHandler errorHandler = null
		)
		{
			//	Resources情報
			ResourcesDatabase = new ResourcesDatabase( path : "Database/resourcesList.json" );
			//	コンテンツカタログ
			LoadDatabase = loadDatabase;
			//	キャッシュ情報
			StorageDatabase = storageDatabase;

			//---------------------------------
			//	ダウンロード機能
			//---------------------------------
			DownloadProvider = downloadProvider;
			//---------------------------------
			//	キャッシュ機能
			//---------------------------------
			StorageProvider = storageProvider;
			//---------------------------------
			//	アセットロード機能
			//---------------------------------
			AssetLoadProvider = assetProvider;
			//---------------------------------
			//	破棄機能
			//---------------------------------
			UnloadProvider = unloadProvider;

			//-----------------------
			// エラー受信機
			//-----------------------
			ErrorHandler = errorHandler;
			if( ErrorHandler != null)
			{
				DownloadProvider.OnDownloadError = code => ErrorHandler.Receive( code );
				AssetLoadProvider.OnError = code => ErrorHandler.Receive( code );
			}
		}

		/// <summary>
		/// 破棄処理
		/// </summary>
		public void Dispose()
		{
			StorageDatabase?.Dispose();
			LoadDatabase?.Dispose();
			ResourcesDatabase?.Dispose();
			DownloadProvider?.Dispose();
			AssetLoadProvider?.Dispose();
			UnloadProvider?.Dispose();
			StorageProvider?.Dispose();
			ErrorHandler?.Dispose();

			StorageDatabase = null;
			LoadDatabase = null;
			ResourcesDatabase = null;
			DownloadProvider = null;
			AssetLoadProvider = null;
			UnloadProvider = null;
			StorageProvider = null;
		}

		/// <summary>
		/// 初期化
		/// </summary>
		public IEnumerator Setup()
		{
			yield return ResourcesDatabase	.InitLoad();
			yield return StorageDatabase	.Initialize();
		}

		/// <summary>
		/// ログイン
		/// </summary>
		public IEnumerator Login( IAccessPoint server )
		{
			yield return UnloadProvider.ForceReleaseAll();
			//	接続先
			AccessServer = server;
			yield return DownloadProvider.InitLoad( AccessServer );
		}

		public void Logout()
		{
			ErrorHandler?.Init();
			DownloadProvider.Cancel();
			AssetLoadProvider?.Cancel();
			UnloadProvider.ForceReleaseAll();
			LoadDatabase.Clear();
		}
		/// <summary>
		///事前ロード処理 
		/// </summary>
		private ILoadProcess DeepDownloadImpl( string assetPath )
		{
			var asset = LoadDatabase.GetAssetData( assetPath );
			if( asset == null)
			{
				return SkipLoadProcess.Default;
			}
			var bundle = asset.BundleData;
			if( bundle.Dependencies.Length == 0 )
			{
				// 1個しかないなら自分だけ
				return SingleDownloadImpl( bundle.Name );
			}
			var prev = bundle
						.Dependencies
						.Where( c => !c.IsOnMemory )
						.Where( c => !StorageDatabase.HasStorage(c))
						.Select(c => SingleDownloadImpl(c.Name))
						.ToArray()
						.ToParallel()
						;
			var job = prev.ToJoin(SingleDownloadImpl( bundle.Name ));

			return job;
		}
		public IPreloadOperation DeepDownload(string assetPath)
		{
			var process = DeepDownloadImpl( assetPath );

			return AssetLoadProvider.Preload( process );
		}

		/// <summary>
		/// バンドル単体のDL
		/// </summary>
		private ILoadProcess SingleDownloadImpl( string abName )
		{
			return DownloadProvider.CacheOrDownload(abName);
		}

		public IPreloadOperation SingleDownload(string abName)
		{
			var process = SingleDownloadImpl( abName );
			return AssetLoadProvider.Preload( process );
		}

		/// <summary>
		/// ファイルオープン
		/// </summary>
		public ILoadProcess DeepOpenFileImpl( string assetPath )
		{
			var asset = LoadDatabase.GetAssetData( assetPath );
			if( asset == null )
			{
				return SkipLoadProcess.Default;
			}
			var bundle = asset.BundleData;
			if (bundle.Dependencies.Length == 0)
			{
				// 1個しかないなら自分だけ
				return SingleOpenFileImpl( bundle.Name );
			}
			//	そうでないなら依存先をDLしてから自分

			var prev = bundle
						.Dependencies
						.Where( d => !d.IsOnMemory )
						.Select(c => SingleOpenFileImpl( c.Name ) )
						.ToArray()
						.ToParallel()
						;
			var job = prev.ToJoin(SingleOpenFileImpl(bundle.Name));

			return job;
		}

		public IPreloadOperation DeepOpenFile( string assetPath )
		{
			var process = DeepOpenFileImpl( assetPath );
			return AssetLoadProvider.Preload( process );
		}
		/// <summary>
		/// 単一ロード
		/// </summary>
		private ILoadProcess SingleOpenFileImpl( string abName)
		{
			return DownloadProvider.LoadFile(abName);
		}
		public IPreloadOperation SingleOpenFile(string abName)
		{
			var process = SingleOpenFileImpl(abName);
			return AssetLoadProvider.Preload( process );
		}


		/// <summary>
		/// アセットアクセス
		/// </summary>
		public IAssetLoadOperation<T> LoadAsset<T>( string assetPath ) where T : UnityEngine.Object
		{
			//	DBにない == Resources はDL処理がいらない
			if (!LoadDatabase.Contains( assetPath ))
			{
				return LoadAssetInternal<T>(assetPath);
			}
			return LoadAssetDownloads<T>(assetPath);
		}

		private IAssetLoadOperation<T> LoadAssetInternal<T>( string assetPath) where T : UnityEngine.Object
		{
			return AssetLoadProvider.LoadAsset<T>(assetPath);
		}

		private IAssetLoadOperation<T> LoadAssetDownloads<T>(string assetPath) where T : UnityEngine.Object
		{
			// DL処理をつくって事前処理に渡す

			var preProcess = new Func<string, ILoadProcess>[]
			{
				p => DeepDownloadImpl( p ),
				p => DeepOpenFileImpl( p ),
			};
			return AssetLoadProvider.LoadAsset<T>(assetPath, preProcess);
		}

		/// <summary>
		/// シーン遷移
		/// </summary>
		public ISceneLoadOperation LoadLevel( string scenePath, LoadSceneMode mode )
		{
			//	DBにない == Resources はDL処理がいらない
			if (!LoadDatabase.Contains(scenePath))
			{
				return LoadLevelInternal(scenePath, mode);
			}
			return LoadLevelDownloads( scenePath, mode );
		}

		private ISceneLoadOperation LoadLevelInternal( string scenePath, LoadSceneMode mode)
		{
			return AssetLoadProvider.LoadLevel(scenePath, mode);
		}

		private ISceneLoadOperation LoadLevelDownloads(string scenePath, LoadSceneMode mode)
		{
			// DL処理をつくって事前処理に渡す
			var preProcess = new Func<string, ILoadProcess>[]
			{
				p => DeepDownloadImpl( p ),
				p => DeepOpenFileImpl( p ),
			};
			return AssetLoadProvider.LoadLevel(scenePath, mode, preProcess);
		}

		/// <summary>
		/// 破棄
		/// </summary>
		public IEnumerator Unload( bool isForceUnloadAll )
		{
			if( isForceUnloadAll )
			{
				yield return UnloadProvider.ForceReleaseAll();
				yield break;
			}
			yield return UnloadProvider.UnloadUnusedAssets();
		}

		/// <summary>
		/// キャッシュクリア
		/// </summary>
		public IEnumerator StorageClear()
		{
			yield return Unload( true );
			yield return StorageProvider.AllClear();
		}

		/// <summary>
		/// 更新
		/// </summary>
		public void DoUpdate()
		{
			if (ErrorHandler?.IsError ?? false)
			{
				//	エラー発生したら止める
				return;
			}
			DownloadProvider?.DoUpdate();
			AssetLoadProvider?.DoUpdate();
			ErrorHandler?.Update();
		}

		/// <summary>
		/// 後処理
		/// </summary>
		public void DoLateUpdate()
		{
		}

		/// <summary>
		/// 
		/// </summary>
		public IEnumerable<TBundleData> GetNeedDownloadList()
		{
			return StorageProvider.GetNeedUpdateList();
		}
		/// <summary>
		/// ぜんぶ
		/// </summary>
		public IEnumerable<TBundleData> GetList()
		{
			return LoadDatabase.BundleList;
		}

		/// <summary>
		/// ファイル検索
		/// </summary>
		public IEnumerable<string> SearchFileList( string searchKey )
		{
			if( ResourcesDatabase == null )
			{
				return new string[ 0 ];
			}
			//	キーナシは全部渡す
			if (string.IsNullOrEmpty(searchKey))
			{
				return _GetAllAssetsPath();
			}
			//	検索する
			return _GetFilterdAssetsPath( searchKey );
		}
		/// <summary>
		/// 全アセットのパス一覧データを取得
		/// </summary>
		/// <returns></returns>
		private IEnumerable<string> _GetAllAssetsPath()
		{
			var resourcesList   = ResourcesDatabase.AssetList;
			if( LoadDatabase == null )
			{
				return resourcesList;
			}
			//	アセットバンドルロード済みなら混合
			return resourcesList.Union( LoadDatabase.AssetList.Select( c => c.Path ) );
		}

		/// <summary>
		/// 絞り込み検索
		/// </summary>
		private IEnumerable<string> _GetFilterdAssetsPath( string key )
		{
			var pattern     = key.Replace( "*","(.*?)" );
			var regex       = new Regex( pattern );
			var searchList  = new List<string>();

			//	Resourcesの中身
			foreach( var p in ResourcesDatabase.AssetList )
			{
				if( searchList.Contains( p ) )
				{
					continue;
				}
				if( regex.IsMatch( p ) )
				{
					searchList.Add( p );
				}
			}
			if( LoadDatabase == null)
			{
				return searchList;
			}

			//	アセットバンドルの中身
			foreach( var p in LoadDatabase.AssetList )
			{
				if( regex.IsMatch( p.Path ) )
				{
					searchList.Add( p.Path );
				}
			}

			return searchList;
		}

		public bool IsCache( string abName )
		{
			if( LoadDatabase == null )
			{
				return false;
			}
			var data = LoadDatabase.GetBundleData( abName );
			if( data == null)
			{
				return false;
			}

			if( !StorageDatabase.HasStorage( data ))
			{
				return false;
			}
			return true;
		}

		/// <summary>
		/// 保存ディレクトリ
		/// </summary>
		public IAccessPoint GetLocalDir()
		{
			return StorageDatabase.GetCacheStorage();
		}

		/// <summary>
		/// 
		/// </summary>
		public void Stop()
		{
			AssetLoadProvider.Cancel();
			DownloadProvider.Cancel();
		}
	}
}
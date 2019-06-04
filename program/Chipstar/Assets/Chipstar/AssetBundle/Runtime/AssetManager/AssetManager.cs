using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

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
			IAssetUnloadProvider unloadProvider
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
		}

		/// <summary>
		/// 破棄処理
		/// </summary>
		public void Dispose()
		{
			StorageDatabase.Dispose();
			LoadDatabase.Dispose();
			ResourcesDatabase.Dispose();
			DownloadProvider.Dispose();
			AssetLoadProvider.Dispose();
			UnloadProvider.Dispose();
			StorageProvider.Dispose();

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
			DownloadProvider.Cancel();
			UnloadProvider.ForceReleaseAll();
			LoadDatabase.Clear();
		}
		/// <summary>
		///事前ロード処理 
		/// </summary>
		public ILoadProcess DeepDownload( string assetPath )
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
				return SingleDownload( bundle.Name );
			}
			var prev = bundle
						.Dependencies
						.Where( c => !c.IsOnMemory )
						.Where( c => !StorageDatabase.HasStorage(c))
						.Select(c => SingleDownload(c.Name))
						.ToArray()
						.ToParallel()
						;
			var job = prev.ToJoin( SingleDownload( bundle.Name ));

			return job;
		}

		/// <summary>
		/// バンドル単体のDL
		/// </summary>
		public ILoadProcess SingleDownload( string abName )
		{
			return DownloadProvider.CacheOrDownload(abName);
		}


		/// <summary>
		/// ファイルオープン
		/// </summary>
		public ILoadProcess DeepOpenFile( string assetPath )
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
				return SingleOpenFile( bundle.Name );
			}
			//	そうでないなら依存先をDLしてから自分

			var prev = bundle
						.Dependencies
						.Where( d => !d.IsOnMemory )
						.Select(c => SingleOpenFile( c.Name ) )
						.ToArray()
						.ToParallel()
						;
			var job = prev.ToJoin(SingleOpenFile(bundle.Name));

			return job;
		}

		/// <summary>
		/// 単一ロード
		/// </summary>
		public ILoadProcess SingleOpenFile( string abName)
		{
			return DownloadProvider.LoadFile(abName);
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
			UnloadProvider.AddRef( scenePath );
			return AssetLoadProvider.LoadLevel( scenePath );
		}

		/// <summary>
		/// シーン加算
		/// </summary>
		public ISceneLoadOperation LoadLevelAdditive( string scenePath )
		{
			UnloadProvider.AddRef( scenePath );
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
				yield return UnloadProvider.ForceReleaseAll();
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
			DownloadProvider?.DoUpdate();
			AssetLoadProvider?.DoUpdate();
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
			DownloadProvider.Cancel();
		}
	}
}
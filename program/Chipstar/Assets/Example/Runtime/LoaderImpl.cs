
//#define EDITOR_SIMULATE_ASSETBUNDLE
//#define EDITOR_SIMULATE_CRI_DOWNLOAD

using Chipstar;
using Chipstar.Downloads;
using Chipstar.Downloads.CriWare;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Pony
{
    /// <summary>
    /// Loaderクラスの内部側
    /// </summary>
    public sealed partial class Loader : MonoBehaviour
    {
        //==============================
        //	enum
        //==============================
        private enum LoginStatus
        {
            None,
            Loading,
            Completed
        }

        //==============================
        //	変数
        //==============================
        private IAssetManager<RuntimeBundleData> m_assetManager = null;
        private ICriSoundFileManager m_criSoundFileManager = null;
        private ICriMovieFileManager m_criMovieFileManager = null;

        private IAccessPoint m_loginServer = null;
        private IManifestLoader m_manifestLoader = null;
        private FileVersionTable m_abVersionTable = null;
        private FileVersionTable m_soundVersionTable = null;
        private FileVersionTable m_movieVersionTable = null;

        private IAssetVersion m_version = null;

        private LoginStatus m_state = LoginStatus.None;
        private bool m_isInit = false;
        private Action m_onLogin = null;

        //=============================================
        // 関数
        //=============================================

        /// <summary>
        /// コンストラクタ
        /// </summary>
        static Loader()
        {
            var go = new GameObject("Loader");
            ms_Instance = go.AddComponent<Loader>();
        }

        public static void Construct() { }

        //--------------------------------------------
        // Unity
        //--------------------------------------------

        private void OnDestroy()
        {
            m_onLogin = null;
            m_criSoundFileManager = null;
            m_criMovieFileManager = null;
            m_assetManager = null;
        }

        private void Start()
        {

        }




        void Update()
        {
            m_assetManager?.DoUpdate();
            m_criSoundFileManager?.DoUpdate();
            m_criMovieFileManager?.DoUpdate();
        }

        private void LateUpdate()
        {
            m_assetManager?.DoLateUpdate();
        }

        /// <summary>
        /// アセットバンドル機能のマネージャインスタンス作成
        /// </summary>
        private void _CreateAssetManagerInstance(IAccessPoint local)
        {

            //	アセットバンドルマネージャの機能構築
            var contentConfig = new ContentGroupConfig();
            var engine = new JobEngine();
            var encoding = new System.Text.UTF8Encoding(false);
            var parser = new CompressedJsonParser<BuildMapDataTable>(encoding);
            var localStorage = local.ToAppend(contentConfig.StorageDirPath);
            var loadDatabase = new LoadDatabase<BuildMapDataTable, BundleBuildData, AssetBuildData, RuntimeBundleData>( parser );

            var storageDatabase = new StorageDatabase<SaveFileTable<LocalBundleData>, LocalBundleData>(
                savePoint: localStorage,
                storageDbName: contentConfig.LocalFileName,
                parser: new RawTextJsonParser<SaveFileTable<LocalBundleData>>(encoding),
                writer: new RawTextJsonWriter<SaveFileTable<LocalBundleData>>(encoding)
            );
            var storageProvider = new StorageProvider<RuntimeBundleData>
                (
                    assetDatabase: loadDatabase,
                    storageDatabase: storageDatabase
                );
            var downloadProvider = new DownloadProvider<RuntimeBundleData>
                (
                    loadDatabase: loadDatabase,
                    storageDatabase: storageDatabase,
                    dlEngine: engine,
                    jobCreator: new WRJobCreator()
                );
            var assetProvider = new AssetLoadProvider
                (
                     new FactoryContainer
                    (
                        assets: new IAssetLoadFactory[]
                        {
                            new AssetBundleLoadFactory<RuntimeBundleData>( loadDatabase ),
                            new ResourcesLoadFactory()
                        },
                        scenes: new ISceneLoadFactory[]
                        {
                            new BuiltInSceneLoadFactory(),
                            new SceneLoadFactory<RuntimeBundleData>( loadDatabase ),
                        }
                    )
                );
            var unloadProvider = new AssetUnloadProvider<RuntimeBundleData>(loadDatabase);


            downloadProvider.GetBuildMapLocation = server =>
            {
                return server
                        .ToAppend(m_abVersionTable.Get(contentConfig.RemoteFileName))
                        .ToAppend(contentConfig.RemoteDirPath)
                        .ToLocation(contentConfig.RemoteFileName);
            };

            downloadProvider.GetBundleLocation = (server, data) =>
            {
                return server
                        .ToAppend(m_abVersionTable.Get(data.Name))
                        .ToAppend(contentConfig.RemoteDirPath)
                        .ToLocation(data.Name);
            };


            //	初期化
            m_assetManager = new AssetManager<RuntimeBundleData>(
                loadDatabase: loadDatabase,
                storageDatabase: storageDatabase,
                downloadProvider: downloadProvider,
                storageProvider: storageProvider,
                assetProvider: assetProvider,
                unloadProvider: unloadProvider
            );
        }

        /// <summary>
        /// Criマネージャのインスタンス作成
        /// </summary>
        private void _CreateCriFileManagerInstance(IAccessPoint downloadStorage, IAccessPoint includeStorage)
        {
            var soundConfig = new ContentGroupConfig();
            var movieConfig = new ContentGroupConfig();
			var streamingAssetsDB = new StreamingAssetsDatabase("Database/streamingAssetsList.json");

            var soundManager = new CriSoundFileManager(
				includeStorage : includeStorage.ToAppend(soundConfig.IncludeDirPath),
				downloadStorage: downloadStorage.ToAppend(soundConfig.StorageDirPath),
				streamingAssetsDatabase:streamingAssetsDB,
                new MultiLineJobEngine( 2 )
			);
			var movieManager = new CriMovieFileManager
				(
					includeStorage : includeStorage.ToAppend( movieConfig.IncludeDirPath ),
					downloadStorage: downloadStorage.ToAppend( movieConfig.StorageDirPath ),
					streamingAssetsDatabase : streamingAssetsDB,
                    new MultiLineJobEngine(2)
                );
			soundManager.GetManifestLocation = server =>
			{

				return server
						.ToAppend( m_soundVersionTable.Get( soundConfig.RemoteFileName ) )
						.ToAppend( soundConfig.RemoteDirPath )
						.ToLocation( soundConfig.RemoteFileName );
			};
			movieManager.GetManifestLocation = server =>
			{
				return server
						.ToAppend( m_movieVersionTable.Get( movieConfig.RemoteFileName ))
						.ToAppend( movieConfig.RemoteDirPath )
						.ToLocation( movieConfig.RemoteFileName );
			};
			soundManager.GetFileDLLocation = (server, relativePath) =>
			{
				return server
						.ToAppend( m_soundVersionTable.Get( relativePath ))
						.ToAppend( soundConfig.RemoteDirPath )
						.ToLocation( relativePath );
			};
			movieManager.GetFileDLLocation = (server, relativePath) =>
			{
				return server
						.ToAppend( m_movieVersionTable.Get( relativePath ) )
						.ToAppend( movieConfig.RemoteDirPath )
						.ToLocation( relativePath );
			};

			m_criSoundFileManager = soundManager;
			m_criMovieFileManager = movieManager;
        }

    }
}
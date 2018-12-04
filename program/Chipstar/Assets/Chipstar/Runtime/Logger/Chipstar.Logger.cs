using System;
using System.Collections;
using System.Collections.Generic;
using Chipstar.Downloads;
using UnityEngine;
using System.Diagnostics;

namespace Chipstar
{
	/// <summary>
	/// ログを管理する部分
	/// </summary>
	public static partial class Chipstar
	{
		//=============================
		//	const
		//=============================
		private const string TAG = "[Chipstar]";

		private const string SIMPLE_LOG_MODE = "CHIPSTAR_ENABLE_LOG";
		private const string DETAIL_LOG_MODE = "CHIPSTAR_ENABLE_DETAIL_LOG";
		//=============================
		//	プロパティ
		//=============================
		private static ILogger _logger = null;
		public static ILogger Logger
		{
			private get { return _logger ?? ( _logger = new Logger( UnityEngine.Debug.unityLogger ) ); }
			set { _logger = value; }
		}
		//=============================
		//	関数
		//=============================

		/// <summary>
		/// ジョブ実行時
		/// </summary>
		[Conditional( DETAIL_LOG_MODE )]
		internal static void Log_RunJob( IAccessLocation location )
		{
			LogCore( string.Format( "Run Job : {0}", location.AccessPath ) );
		}
		/// <summary>
		/// ジョブ進行時
		/// </summary>
		[Conditional( DETAIL_LOG_MODE )]
		internal static void Log_UpdateJob<TSource>( TSource source )
		{
			LogCore( string.Format( "Update Job : {0}", source.ToString() ) );
		}
		/// <summary>
		/// ジョブ完了時
		/// </summary>
		[Conditional( DETAIL_LOG_MODE )]
		internal static void Log_DoneJob<TSource>( TSource source, IAccessLocation location )
		{
			LogCore( string.Format( "Done Job : {0} : {1}", source.ToString(), location.AccessPath ) );
		}
		/// <summary>
		/// 取得先の作成
		/// </summary>
		[Conditional( DETAIL_LOG_MODE )]
		[Conditional( SIMPLE_LOG_MODE )]

		internal static void Log_CreateEntryPoint( string result )
		{
			LogCore( string.Format( "EntryPoint : {0}", result ) );
		}
		/// <summary>
		/// ジョブの登録
		/// </summary>
		[Conditional( DETAIL_LOG_MODE )]
		internal static void Log_AddJob( ILoadJob job )
		{
			LogCore( string.Format( "Enqueue : {0}", job.ToString() ) );
		}

		/// <summary>
		/// 読み込み開始
		/// </summary>
		[Conditional( DETAIL_LOG_MODE )]
		[Conditional( SIMPLE_LOG_MODE )]
		internal static void Log_LoadStart( string path )
		{
			LogCore( string.Format( "Start Download : {0}", path ) );
		}

		/// <summary>
		/// 対象Fileが見つからない
		/// </summary>
		[Conditional( DETAIL_LOG_MODE )]
		[Conditional( SIMPLE_LOG_MODE )]
		internal static void Log_AssetNotFound( string path )
		{
			WarningCore( string.Format( "Not Found From Table : {0}", path ) );
		}

		/// <summary>
		/// アセットデータベースの初期化開始
		/// </summary>
		[Conditional( DETAIL_LOG_MODE )]
		internal static void Log_Downloader_StartInit()
		{
			LogCore( "InitTable" );
		}

		/// <summary>
		/// 
		/// </summary>
		[Conditional( DETAIL_LOG_MODE )]
		[Conditional( SIMPLE_LOG_MODE )]
		internal static void Log_Downloader_RequestBuildMap( IAccessLocation location )
		{
			LogCore( string.Format( "Get Request : {0}", location.AccessPath ) );
		}
		/// <summary>
		/// アセットデータベースの取得
		/// </summary>
		[Conditional( DETAIL_LOG_MODE )]
		internal static void Log_GetBuildMap<TTable, TBundle, TAsset>( TTable table )
			where TTable : IBuildMapDataTable<TBundle, TAsset>
			where TBundle : IBundleBuildData
			where TAsset : IAssetBuildData
		{
			LogCore( string.Format( "Serialized : {0}", table.ToString() ) );
		}

		/// <summary>
		/// キャッシュデータベースの初期化
		/// </summary>
		[Conditional( DETAIL_LOG_MODE )]
		[Conditional( SIMPLE_LOG_MODE )]
		internal static void Log_InitCacheDB( string path )
		{
			LogCore( string.Format( "Get CacheDB : {0}", path ) );
		}
		/// <summary>
		/// キャッシュデータベースの取得
		/// </summary>
		[Conditional( DETAIL_LOG_MODE )]
		internal static void Log_InitCacheDB_ReadLocalFile( IEnumerable<LocalBundleData> table )
		{
			LogCore( string.Format( "Serialized : {0}", table.ToString() ) );
		}
		/// <summary>
		/// キャッシュデータベースの初回作成
		/// </summary>
		[Conditional( DETAIL_LOG_MODE )]
		internal static void Log_InitCacheDB_FirstCreate( string path )
		{
			LogCore( string.Format( "First Create : {0}", path ) );
		}
		[Conditional( DETAIL_LOG_MODE )]
		internal static void Log_WriteLocalBundle( IAccessLocation location )
		{
			LogCore( string.Format( "Write Cache File : {0}", location.AccessPath ) );
		}
		[Conditional( DETAIL_LOG_MODE )]
		internal static void Log_SaveLocalVersion( ICachableBundle data )
		{
			LogCore( string.Format( "Save File Version : {0}", data.ToString() ) );
		}
		[Conditional( DETAIL_LOG_MODE )]
		internal static void Log_DeleteLocalBundle( ICachableBundle data )
		{
			LogCore( string.Format( "Delete Cache File : {0}", data.Name ) );
		}
		[Conditional( DETAIL_LOG_MODE )]
		internal static void Log_RemoveLocalVersion( ICachableBundle data )
		{
			LogCore( string.Format( "Delete File Version : {0}", data.ToString() ) );
		}
		[Conditional( DETAIL_LOG_MODE )]
		internal static void Log_ApplyLocalSaveFile( string path )
		{
			LogCore( string.Format( "Apply Cache SaveData : {0}", path ) );
		}
		[Conditional( DETAIL_LOG_MODE )]
		[Conditional( SIMPLE_LOG_MODE )]
		internal static void Log_LoadAsseT<T>( string path, IAssetLoadFactory factory ) where T : UnityEngine.Object
		{
			LogCore( string.Format( "Asset Load {0}({1}) => {2}", path, typeof( T ), typeof( IAssetLoadFactory ).Name ) );
		}
		[Conditional( DETAIL_LOG_MODE )]
		[Conditional( SIMPLE_LOG_MODE )]
		internal static void Log_LoadLevel( string path, ISceneLoadFactory factory )
		{
			LogCore( string.Format( "Load Scene {0} => {1}", path, typeof( IAssetLoadFactory ).Name ) );
		}
		[Conditional( DETAIL_LOG_MODE )]
		[Conditional( SIMPLE_LOG_MODE )]
		internal static void Log_LoadLevelAdditive( string path, ISceneLoadFactory factory )
		{
			LogCore( string.Format( "Load Scene Additive {0} => {1}", path, typeof( IAssetLoadFactory ).Name ) );
		}
		/// <summary>
		/// ログ表示
		/// </summary>
		[Conditional( DETAIL_LOG_MODE )]
		[Conditional( SIMPLE_LOG_MODE )]
		[DebuggerHidden, DebuggerStepThrough]
		private static void LogCore( string message )
		{
			Logger.Log( LogType.Log, TAG, message );
		}
		[Conditional( DETAIL_LOG_MODE )]
		[Conditional( SIMPLE_LOG_MODE )]
		[DebuggerHidden, DebuggerStepThrough]
		private static void WarningCore( string message )
		{
			Logger.Log( LogType.Warning, TAG, message );
		}
		
	}
}
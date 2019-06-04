using System;
using System.Collections;
using System.Collections.Generic;
using Chipstar.Downloads;
using UnityEngine;
using System.Diagnostics;
using System.IO;
using Chipstar.Downloads.CriWare;

namespace Chipstar
{
	/// <summary>
	/// ログを管理する部分
	/// </summary>
	public static partial class Chipstar
	{
		//=============================
		//	enum
		//=============================
		public enum LogLevel
		{
			None,
			Simple,
			Detail,
			DeepDetail,
		}

		//=============================
		//	const
		//=============================
		private const string TAG = "[Chipstar]";
		private const string ENABLE_CHIPSTAR_LOG = "CHIPSTAR_ENABLE_LOG";
		private const long	 MB  = 1024 * 1024;

		//=============================
		//	プロパティ
		//=============================
		private static ILogger _logger = null;
		public static ILogger Logger
		{
			private get { return _logger ?? ( _logger = new Logger( UnityEngine.Debug.unityLogger ) ); }
			set { _logger = value; }
		}
		private static bool EnableLog		{ get { return LogLevelMode != LogLevel.None	; } }
		private static bool EnableLogDetail { get { return LogLevelMode == LogLevel.Detail	; } }
		private static bool EnableLogDeep { get { return LogLevelMode == LogLevel.DeepDetail; } }
		public static LogLevel LogLevelMode { get; set; } = LogLevel.None;


		//=============================
		//	関数
		//=============================

		/// <summary>
		/// ジョブ実行時
		/// </summary>
		[Conditional( ENABLE_CHIPSTAR_LOG )]
		internal static void Log_RunJob( IAccessLocation location )
		{
			LogDetail( string.Format( "Run Job : {0}", location.FullPath ) );
		}
		/// <summary>
		/// ジョブ進行時
		/// </summary>
		[Conditional( ENABLE_CHIPSTAR_LOG )]
		internal static void Log_UpdateJob<TSource>( TSource source )
		{
			LogDeepDetail( string.Format( "Update Job : {0}", source.ToString() ) );
		}
		/// <summary>
		/// ジョブ完了時
		/// </summary>
		[Conditional( ENABLE_CHIPSTAR_LOG )]
		internal static void Log_DoneJob<TSource>( TSource source, IAccessLocation location )
		{
			LogDetail( string.Format( "Done Job : {0} : {1}", source.ToString(), location.FullPath ) );
		}

		/// <summary>
		/// ログイン処理時
		/// </summary>
		[Conditional( ENABLE_CHIPSTAR_LOG )]
		internal static void Log_Login( IAccessPoint server )
		{
			LogSimple( $"Server : { server.ToString() }" );
		}
		/// <summary>
		/// ジョブの登録
		/// </summary>
		[Conditional( ENABLE_CHIPSTAR_LOG )]
		internal static void Log_AddJob( ILoadJob job )
		{
			LogDeepDetail( string.Format( "Enqueue : {0}", job.ToString() ) );
		}

		[Conditional(ENABLE_CHIPSTAR_LOG)]
		internal static void Log_RequestVersionManifest(IAccessLocation manifestLocation)
		{
			LogDetail(string.Format("Request Ver Manifest : {0}", manifestLocation.FullPath ));
		}
		[Conditional(ENABLE_CHIPSTAR_LOG)]
		internal static void Log_DLVersionManifest( string json )
		{
			LogDetail(json);
		}

		/// <summary>
		/// アセットデータベースの初期化開始
		/// </summary>
		[Conditional( ENABLE_CHIPSTAR_LOG )]
		internal static void Log_Downloader_StartInit()
		{
			LogSimple( "InitTable" );
		}

		/// <summary>
		/// 
		/// </summary>
		[Conditional( ENABLE_CHIPSTAR_LOG ) ]
		internal static void Log_Downloader_RequestBuildMap( IAccessLocation location )
		{
			LogSimple( string.Format( "Get Request : {0}", location.FullPath ) );
		}

		/// <summary>
		/// データベースが空データだった
		/// </summary>
		[Conditional( ENABLE_CHIPSTAR_LOG )]
		internal static void Log_Database_NotFound( )
		{
			WarningSimple( "Database NotFound " );
		}

		[Conditional( ENABLE_CHIPSTAR_LOG )]
		internal static void Log_WriteLocalBundle( IAccessLocation location, byte[] content )
		{
			if( content == null || content.Length == 0 )
			{
				WarningSimple( string.Format( "Write File Error : {0}", location.FullPath ) );
				return;
			}
			LogSimple( string.Format( "Write Cache File : {0} [Size = {1} byte]", location.FullPath, content.Length ) );
		}

		public static void AssertNotNull<T>(T obj, string message) where T : class
		{
			if (obj != null)
			{
				return;
			}
			AssertSimple(message);
		}

		[Conditional( ENABLE_CHIPSTAR_LOG )]
		internal static void Log_WriteLocalBundleError( IAccessLocation location )
		{
			AssertSimple( string.Format( "Write File Error : {0} ", location.FullPath ) );
		}

		[Conditional( ENABLE_CHIPSTAR_LOG )]
		internal static void Log_Parse_Result( string json )
		{
			if( json == null || json.Length==0 )
			{
				WarningSimple( "Database String Parse Error" );
				return;
			}
			LogSimple( json );
		}

		[Conditional( ENABLE_CHIPSTAR_LOG )]
		internal static void Log_ApplyLocalSaveFile( string path )
		{
			LogSimple( string.Format( "Apply Cache SaveData : {0}", path ) );
		}
		[Conditional( ENABLE_CHIPSTAR_LOG )]
		internal static void Log_StartOperation<T>( T loadOperation ) where T : ILoadOperation
		{
			LogSimple( string.Format( "Load Async {0}({1}) ", typeof( T ), loadOperation.ToString() ) );
		}

		[Conditional( ENABLE_CHIPSTAR_LOG )]
		internal static void Log_WarningNotHasExtensions( string path )
		{
			WarningSimple( string.Format( "Not Has Extensions : {0}", path ) );
		}

		[Conditional( ENABLE_CHIPSTAR_LOG )]
		internal static void Log_WarningAccessAfterLoginAsset( string path )
		{
			WarningSimple( string.Format( "Can Access Only EditorMode : {0}", path ) );
		}

		[Conditional(ENABLE_CHIPSTAR_LOG)]
		[DebuggerHidden, DebuggerStepThrough]
		internal static void Log_MaybeFileBreak( FileInfo info, long previewSize )
		{
			WarningSimple($"MayBe File Break : {info?.FullName ?? "Empty" }[ DLSize:{previewSize}byte <=> CacheFileSize:{ info?.Length ?? 0 }byte]");
		}
		[Conditional(ENABLE_CHIPSTAR_LOG)]
		[DebuggerHidden, DebuggerStepThrough]
		internal static void Log_MissMatchVersion( string key, string oldVersion, string newVersion )
		{
			LogDetail($"MissMatch Version : {key} [{oldVersion}->{newVersion}]");
		}

		/// <summary>
		/// ログ表示
		/// </summary>
		[Conditional( ENABLE_CHIPSTAR_LOG ) ]
		[DebuggerHidden, DebuggerStepThrough]
		private static void LogSimple( string message )
		{
			if( !EnableLog ) { return; }
			Logger.Log( LogType.Log, TAG, message );
		}
		[Conditional( ENABLE_CHIPSTAR_LOG )]
		[DebuggerHidden, DebuggerStepThrough]
		private static void LogDetail( string message )
		{
			if( !EnableLogDetail ) { return; }
			Logger.Log( LogType.Log, TAG, message );
		}
		[Conditional(ENABLE_CHIPSTAR_LOG)]
		[DebuggerHidden, DebuggerStepThrough]
		private static void LogDeepDetail(string message)
		{
			if (!EnableLogDeep) { return; }
			Logger.Log(LogType.Log, TAG, message);
		}

		[Conditional( ENABLE_CHIPSTAR_LOG ) ]
		[DebuggerHidden, DebuggerStepThrough]
		private static void WarningSimple( string message )
		{
			if( !EnableLog ) { return; }
			Logger.Log( LogType.Warning, TAG, message );
		}
		[Conditional( ENABLE_CHIPSTAR_LOG )]
		[DebuggerHidden, DebuggerStepThrough]
		private static void WarningDetail( string message )
		{
			if( !EnableLogDetail ) { return; }
			Logger.Log( LogType.Warning, TAG, message );
		}
		[Conditional(ENABLE_CHIPSTAR_LOG)]
		[DebuggerHidden, DebuggerStepThrough]
		private static void AssertDetail(string message)
		{
			if (!EnableLogDetail) { return; }
			Logger.Log(LogType.Assert, TAG, message);
		}
		private static void AssertSimple(string message)
		{
			if (!EnableLog) { return; }
			Logger.Log(LogType.Assert, TAG, message);
		}
	}
}
using System;
using System.Collections;
using System.Collections.Generic;
using Chipstar.Downloads;
using UnityEngine;
using System.Diagnostics;
using System.IO;
using Chipstar.Downloads.CriWare;
using System.Text;

namespace Chipstar
{
	/// <summary>
	/// ログを管理する部分
	/// </summary>
	public static partial class ChipstarLog
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
		//	変数
		//=============================
		private static StringBuilder m_builder = new StringBuilder(500);

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

		[Conditional( ENABLE_CHIPSTAR_LOG )]
		internal static void Log_Run<T>( T source )
		{
			if (!EnableLog) { return; }
			Log($"<color=green>[ Run ]{source?.ToString() ?? string.Empty}</color>");
		}
		[Conditional( ENABLE_CHIPSTAR_LOG )]
		internal static void Log_Update<TSource>( TSource source )
		{
			if (!EnableLogDetail) { return; }
			Log($"[ Update ]{source?.ToString() ?? string.Empty}");
		}
		[Conditional(ENABLE_CHIPSTAR_LOG)]
		internal static void Log_Cancel<T>(T source)
		{
			if (!EnableLog) { return; }
			Log($"<color=yellow>[ Cancel ]{source?.ToString() ?? string.Empty}</color>");
		}
		[Conditional( ENABLE_CHIPSTAR_LOG )]
		internal static void Log_Done<TSource>( TSource source )
		{
			if (!EnableLogDetail) { return; }
			Log($"[ Done ]{source?.ToString() ?? string.Empty}");
		}

		internal static void Log_Dispose<T>(T source)
		{
			if (!EnableLog) { return; }
			Log($"<color=cyan>[ Dispose ]{source?.ToString() ?? string.Empty}</color>");
		}
		internal static void Log_Error<TSource>(TSource source)
		{
			if (!EnableLog) { return; }
			Log($"<color=red>[ Error ]{source?.ToString() ?? string.Empty}</color>");
		}

		/// <summary>
		/// ログイン処理時
		/// </summary>
		[Conditional( ENABLE_CHIPSTAR_LOG )]
		internal static void Log_Login( IAccessPoint server )
		{
			if (!EnableLogDetail) { return; }
			Log( $"Server : { server.ToString() }" );
		}
		/// <summary>
		/// ジョブの登録
		/// </summary>
		[Conditional( ENABLE_CHIPSTAR_LOG )]
		internal static void Log_AddJob( ILoadJob job )
		{
			if (!EnableLogDetail) { return; }
			Log( string.Format( "Enqueue : {0}", job.ToString() ) );
		}

		[Conditional(ENABLE_CHIPSTAR_LOG)]
		internal static void Log_RequestVersionManifest(IAccessLocation manifestLocation)
		{
			if (!EnableLogDetail) { return; }
			Log(string.Format("Request Ver Manifest : {0}", manifestLocation.FullPath ));
		}
		[Conditional(ENABLE_CHIPSTAR_LOG)]
		internal static void Log_DLVersionManifest( string json )
		{
			if (!EnableLogDetail) { return; }
			Log(json);
		}

		/// <summary>
		/// アセットデータベースの初期化開始
		/// </summary>
		[Conditional( ENABLE_CHIPSTAR_LOG )]
		internal static void Log_Downloader_StartInit()
		{
			Log( "InitTable" );
		}

		/// <summary>
		/// 
		/// </summary>
		[Conditional( ENABLE_CHIPSTAR_LOG ) ]
		internal static void Log_Downloader_RequestBuildMap( IAccessLocation location )
		{
			Log( string.Format( "Get Request : {0}", location.FullPath ) );
		}

		/// <summary>
		/// データベースが空データだった
		/// </summary>
		[Conditional( ENABLE_CHIPSTAR_LOG )]
		internal static void Log_Database_NotFound( )
		{
			Warning( "Database NotFound " );
		}

		[Conditional( ENABLE_CHIPSTAR_LOG )]
		internal static void Log_WriteLocalBundle( IAccessLocation location, byte[] content )
		{
			if( content == null || content.Length == 0 )
			{
				Warning( string.Format( "Write File Error : {0}", location.FullPath ) );
				return;
			}
			Log( string.Format( "Write Cache File : {0} [Size = {1} byte]", location.FullPath, content.Length ) );
		}

		public static void AssertNotNull<T>(T obj, string message) where T : class
		{
			if (obj != null)
			{
				return;
			}
			Assert(message);
		}

		[Conditional(ENABLE_CHIPSTAR_LOG)]
		internal static void Log_Dump_RefLog<T>(AssetData<T> data) where T : IRuntimeBundleData<T>
		{
			if (!EnableLogDeep) { return; }
			if (data == null)
			{
				return;
			}
			var bundle = data.BundleData;
			m_builder.Length = 0;
			m_builder.Append(bundle.Name).Append(":").Append(bundle.RefCount).AppendLine();
			foreach ( var d in bundle.Dependencies )
			{
				m_builder
					.Append("---")
					.Append(d.Name)
					.Append(":")
					.Append(d.RefCount)
					.AppendLine();
			}

			Log(m_builder.ToString());
		}

		[Conditional(ENABLE_CHIPSTAR_LOG)]
		internal static void Log_Unload_Bundle<T>(BundleData<T> bundleData) where T : IRuntimeBundleData<T>
		{
			if (!EnableLogDetail) { return; }
			Log($"{bundleData?.Name} Unload");
		}

		[Conditional(ENABLE_CHIPSTAR_LOG)]
		internal static void Log_DisposeUnused<T>(IReadOnlyList<T> freeList) where T : IRuntimeBundleData<T>
		{
			if (!EnableLogDetail) { return; }
			m_builder.Length = 0;
			m_builder.AppendLine("[ UnUsed Dispose ]");
			foreach( var b in freeList )
			{
				m_builder.AppendLine( b.Name );
			}
			Warning(m_builder.ToString());
		}

		[Conditional( ENABLE_CHIPSTAR_LOG )]
		internal static void Log_WriteLocalBundleError( IAccessLocation location )
		{
			Assert( string.Format( "Write File Error : {0} ", location.FullPath ) );
		}

		[Conditional( ENABLE_CHIPSTAR_LOG )]
		internal static void Log_Parse_Result( string json )
		{
			if( json == null || json.Length==0 )
			{
				Warning( "Database String Parse Error" );
				return;
			}
			Log( json );
		}

		[Conditional( ENABLE_CHIPSTAR_LOG )]
		internal static void Log_ApplyLocalSaveFile( string path )
		{
			Log( string.Format( "Apply Cache SaveData : {0}", path ) );
		}
		[Conditional( ENABLE_CHIPSTAR_LOG )]
		internal static void Log_StartOperation<T>( T loadOperation ) where T : ILoadOperation
		{
			Log( string.Format( "Load Async {0}({1}) ", typeof( T ), loadOperation.ToString() ) );
		}

		[Conditional( ENABLE_CHIPSTAR_LOG )]
		internal static void Log_WarningNotHasExtensions( string path )
		{
			Warning( string.Format( "Not Has Extensions : {0}", path ) );
		}

		[Conditional( ENABLE_CHIPSTAR_LOG )]
		internal static void Log_WarningAccessAfterLoginAsset( string path )
		{
			Warning( string.Format( "Can Access Only EditorMode : {0}", path ) );
		}

		[Conditional(ENABLE_CHIPSTAR_LOG)]
		[DebuggerHidden, DebuggerStepThrough]
		internal static void Log_MaybeFileBreak( FileInfo info, long previewSize )
		{
			Warning($"MayBe File Break : {info?.FullName ?? "Empty" }[ DLSize:{previewSize}byte <=> CacheFileSize:{ info?.Length ?? 0 }byte]");
		}
		[Conditional(ENABLE_CHIPSTAR_LOG)]
		[DebuggerHidden, DebuggerStepThrough]
		internal static void Log_MissMatchVersion( string key, string oldVersion, string newVersion )
		{
			if (!EnableLogDetail) { return; }
			Log($"MissMatch Version : {key} [{oldVersion}->{newVersion}]");
		}

		/// <summary>
		/// ログ表示
		/// </summary>
		[Conditional( ENABLE_CHIPSTAR_LOG ) ]
		[DebuggerHidden, DebuggerStepThrough]
		private static void Log( string message )
		{
			if( !EnableLog ) { return; }
			Logger.Log( LogType.Log, TAG, message );
		}
		[Conditional( ENABLE_CHIPSTAR_LOG ) ]
		[DebuggerHidden, DebuggerStepThrough]
		private static void Warning( string message )
		{
			if( !EnableLog ) { return; }
			Logger.Log( LogType.Warning, TAG, message );
		}
		[Conditional(ENABLE_CHIPSTAR_LOG)]
		[DebuggerHidden, DebuggerStepThrough]
		private static void Assert(string message)
		{
			if (!EnableLog) { return; }
			Logger.Log(LogType.Assert, TAG, message);
		}
	}
}
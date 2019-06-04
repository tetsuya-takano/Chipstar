using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Chipstar.Downloads.CriWare;
using Chipstar.Downloads;
using System.Diagnostics;

namespace Chipstar
{
	/// <summary>
	/// Cri絡み用のLogger
	/// </summary>
	public static partial class Chipstar
	{
		/// <summary>
		/// セーブデータ情報ログ
		/// </summary>
		[Conditional(ENABLE_CHIPSTAR_LOG)]
		internal static void Log_ReadLocalTable(CriVersionTableJson cacheDB, IAccessLocation location)
		{
			if (cacheDB == null)
			{
				AssertSimple($"CRI Local Database Is Null :: { location.FullPath }");
				return;
			}
			LogSimple($"Read Cache Info Success:{location.FullPath}");
			LogDetail($"{cacheDB.ToString()}");
		}

		/// <summary>
		/// キューシート存在しない
		/// </summary>
		[Conditional(ENABLE_CHIPSTAR_LOG)]
		internal static void Log_RequestCueSheet_Error( string cueSheetName )
		{
			AssertSimple( cueSheetName );
		}
		/// <summary>
		/// サウンドDLする
		/// </summary>
		[Conditional(ENABLE_CHIPSTAR_LOG)]
		internal static void Log_Download_Sound( ISoundFileData fileData )
		{
			LogDetail($" Start Sound Download : { fileData.CueSheetName } : {fileData.AcbPath},{fileData.AwbPath}");
		}

		/// </summary>
		[Conditional(ENABLE_CHIPSTAR_LOG)]
		internal static void Log_NotContains_RemoteDB_Sound( string cueSheetName )
		{
			WarningDetail($" Not Contains RemoteDB : { cueSheetName  }");
		}
		/// </summary>
		[Conditional(ENABLE_CHIPSTAR_LOG)]
		internal static void Log_NotContains_LocalDB_Sound(string cueSheetName)
		{
			WarningDetail($" Not Contains LocalDB : { cueSheetName  }");
		}
		/// </summary>
		[Conditional(ENABLE_CHIPSTAR_LOG)]
		internal static void Log_NotFound_Downloaded_File( string path )
		{
			WarningDetail( $" File Not Exists : { path }");
		}
	}
}
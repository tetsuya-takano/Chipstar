using Chipstar.Downloads;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Chipstar
{
	public static partial class ChipstarLog
	{
		[Conditional(ENABLE_CHIPSTAR_LOG)]
		internal static void Log_LoadAsset<T>(string path, IAssetLoadFactory factory) where T : UnityEngine.Object
		{
			LogSimple(string.Format("Asset Load {0}({1}) => {2}", path, typeof(T), factory.GetType().Name));
		}
		[Conditional(ENABLE_CHIPSTAR_LOG)]
		internal static void Log_LoadLevel(string path, ISceneLoadFactory factory)
		{
			LogSimple(string.Format("Load Scene {0} => {1}", path, factory.GetType().Name));
		}
		[Conditional(ENABLE_CHIPSTAR_LOG)]
		internal static void Log_GetAssetDatabase(string path)
		{
			LogDetail(string.Format("Get Database Asset = {0}", path));
		}

		[Conditional(ENABLE_CHIPSTAR_LOG)]
		internal static void Log_LoadLevelAdditive(string path, ISceneLoadFactory factory)
		{
			LogSimple(string.Format("Load Scene Additive {0} => {1}", path, factory.GetType().Name));
		}

		[Conditional(ENABLE_CHIPSTAR_LOG)]
		internal static void Log_SaveLocalVersion(ICachableBundle data)
		{
			LogSimple(string.Format("Save File Version : {0}", data.ToString()));
		}
		[Conditional(ENABLE_CHIPSTAR_LOG)]
		internal static void Log_DeleteLocalBundle(ICachableBundle data)
		{
			LogSimple(string.Format("Delete Cache File : {0}", data.Path));
		}
		[Conditional(ENABLE_CHIPSTAR_LOG)]
		internal static void Log_RemoveLocalVersion(ICachableBundle data)
		{
			LogSimple(string.Format("Delete File Version : {0}", data.ToString()));
		}

		[Conditional(ENABLE_CHIPSTAR_LOG)]
		internal static void Log_RequestDownload<TRuntimeData>(TRuntimeData data) where TRuntimeData : IRuntimeBundleData<TRuntimeData>
		{
			LogSimple(string.Format("Download : {0} = {1}MB", data.Name, data.FileSize / MB));
		}
		/// <summary>
		/// キャッシュデータベースの初期化
		/// </summary>
		[Conditional(ENABLE_CHIPSTAR_LOG)]
		internal static void Log_InitStorageDB(string path)
		{
			LogSimple(string.Format("Get CacheDB : {0}", path));
		}
		/// <summary>
		/// キャッシュデータベースの取得
		/// </summary>
		[Conditional(ENABLE_CHIPSTAR_LOG)]
		internal static void Log_InitStorageDB_ReadLocalFile(IEnumerable<LocalBundleData> table)
		{
			LogSimple(string.Format("Serialized : {0}", table.ToString()));
		}
		/// <summary>
		/// キャッシュデータベースの初回作成
		/// </summary>
		[Conditional(ENABLE_CHIPSTAR_LOG)]
		internal static void Log_InitStorageDB_FirstCreate(string path)
		{
			LogSimple(string.Format("First Create : {0}", path));
		}

		[Conditional(ENABLE_CHIPSTAR_LOG)]
		internal static void Log_Skip_OnMemory(string name)
		{
			LogSimple(string.Format("Skip OnMemory {0}", name));
		}

		/// <summary>
		/// アセットデータベースの取得
		/// </summary>
		[Conditional(ENABLE_CHIPSTAR_LOG)]
		internal static void Log_GetBuildMap<TTable, TBundle, TAsset>(TTable table)
			where TTable : IBuildMapDataTable<TBundle, TAsset>
			where TBundle : IBundleBuildData
			where TAsset : IAssetBuildData
		{
			if (table == null)
			{
				WarningSimple("Database Json Parse Error");
				return;
			}
			LogSimple(string.Format("Serialized : {0}", table.ToString()));
		}

		/// <summary>
		/// 
		/// </summary>
		[Conditional(ENABLE_CHIPSTAR_LOG)]
		internal static void Log_Unload_Error<T>(BundleData<T> bundleData) where T : IRuntimeBundleData<T>
		{
			WarningSimple(string.Format("Can't Unload. Reference Somewhere : {0},count={1}", bundleData.Name, bundleData.RefCount));
		}

	}
}
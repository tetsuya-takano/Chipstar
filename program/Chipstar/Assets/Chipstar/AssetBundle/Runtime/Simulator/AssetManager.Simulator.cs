#if UNITY_EDITOR
using Chipstar.Downloads;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Chipstar.Downloads
{
	/// <summary>
	/// シミュレータモード用クラス
	/// </summary>
	public sealed class AssetManagerEditorSimulator : IAssetManager<RuntimeBundleData>
	{
		//====================================
		//	変数
		//====================================
		//====================================
		//	プロパティ
		//====================================
		private IAccessPoint LocalDir { get; set; }
		private AssetLoadSimulator AssetProvider { get; set; }
		private string PrefixPath { get; set; }
		//====================================
		//	関数
		//====================================

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public AssetManagerEditorSimulator(string assetAccessPrefix, IAccessPoint local)
		{
			PrefixPath = assetAccessPrefix.Replace("\\", "/");
			AssetProvider = new AssetLoadSimulator(assetAccessPrefix);
			LocalDir = local;
		}

		/// <summary>
		/// 破棄処理
		/// </summary>
		public void Dispose()
		{
			AssetProvider.Dispose();
		}
		/// <summary>
		/// 初期化
		/// </summary>
		public IEnumerator Setup()
		{
			//	特にナシ
			yield return null;
		}

		public IEnumerator Login(IAccessPoint server )
		{
			ChipstarLog.Log_Login(server);
			yield return null;
			AssetProvider.SetLoginMode(true);
		}

		public void Logout()
		{
			AssetProvider.SetLoginMode(false);
		}

		/// <summary>
		/// 事前ロード
		/// </summary>
		public IPreloadOperation DeepDownload(string path)
		{
			return AssetProvider.Preload(SkipLoadProcess.Default);
		}
		public IPreloadOperation SingleDownload(string name)
		{
			return AssetProvider.Preload(SkipLoadProcess.Default);
		}
		public IPreloadOperation DeepOpenFile(string path)
		{
			return AssetProvider.Preload(SkipLoadProcess.Default);
		}
		public IPreloadOperation SingleOpenFile(string name)
		{
			return AssetProvider.Preload(SkipLoadProcess.Default);
		}

		public IAssetLoadOperation<T> LoadAsset<T>(string assetPath) where T : UnityEngine.Object
		{
			return AssetProvider.LoadAsset<T>(assetPath);
		}

		/// <summary>
		/// シーン遷移
		/// </summary>
		public ISceneLoadOperation LoadLevel(string scenePath, LoadSceneMode mode )
		{
			return AssetProvider.LoadLevel(scenePath, mode );
		}

		/// <summary>
		/// 更新処理
		/// </summary>
		public void DoUpdate()
		{
			AssetProvider.DoUpdate();
		}
		public void DoLateUpdate() { }
		/// <summary>
		/// 
		/// </summary>
		/// <summary>
		/// 破棄
		/// </summary>
		public IEnumerator Unload(bool isForceUnloadAll)
		{
			if (isForceUnloadAll)
			{
				yield return Resources.UnloadUnusedAssets();
			}
			yield return Resources.UnloadUnusedAssets();
		}

		/// <summary>
		/// クリア
		/// </summary>
		public IEnumerator StorageClear()
		{
			yield return null;
		}

		/// <summary>
		/// ファイル検索
		/// </summary>
		public IEnumerable<string> SearchFileList(string searchKey)
		{
			//	対象ファイル一覧
			var prjFileList = UnityEditor
								.AssetDatabase
								.GetAllAssetPaths()
								.Select(c => c.Replace(PrefixPath, string.Empty))
								.ToArray();
			if (string.IsNullOrEmpty(searchKey))
			{
				return prjFileList;
			}

			var pattern = searchKey.Replace("*", "(.*?)");
			var regex = new Regex(pattern);
			var searchList = new List<string>();

			foreach (var p in prjFileList)
			{
				if (regex.IsMatch(p))
				{
					searchList.Add(p);
				}
			}
			return searchList;
		}

		public IEnumerable<RuntimeBundleData> GetNeedDownloadList()
		{
			return new RuntimeBundleData[0];
		}

		public IEnumerable<RuntimeBundleData> GetList()
		{
			return new RuntimeBundleData[0];
		}

		public bool IsCache(string abName)
		{
			return true;
		}

		public IAccessPoint GetLocalDir()
		{
			return LocalDir;
		}

		public void Stop()
		{
			AssetProvider.Cancel();
		}
	}
}
#endif
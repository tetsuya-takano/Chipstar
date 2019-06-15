using UnityEngine;
using System.Collections;
using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.IO;

namespace Chipstar.Downloads
{
	/// <summary>
	/// キャッシュデータ
	/// </summary>
	public interface IStorageDatabase : IDisposable
	{
		IEnumerator Initialize();
		IAccessLocation ToLocation( ICachableBundle data );

		bool HasStorage(ICachableBundle data);
		void Save(ICachableBundle data);
		void Apply();
		void Delete(ICachableBundle bundle);
		void CleanUp();

		IEnumerable<ICachableBundle> GetCachedList();

		IAccessPoint GetCacheStorage();
	}
	/// <summary>
	/// 内部ストレージの管理
	/// </summary>
	public class StorageDatabase : IStorageDatabase
	{
		
		//===============================================
		//  変数
		//===============================================
		private string m_fileName = null;
		private IAccessPoint m_entryPoint = null;
		private IAccessLocation m_versionFile = null;
		private Table m_table = null;

		//===============================================
		//  プロパティ
		//===============================================
		public Func<ICachableBundle, bool> OnSaveVersion { private get; set; }

		//===============================================
		//  関数
		//===============================================

		public StorageDatabase(IAccessPoint savePoint, string storageDbName, Encoding encoding)
		{
			m_entryPoint = savePoint;
			m_fileName = storageDbName;
			m_encoding = encoding;
		}

		/// <summary>
		/// 破棄処理
		/// </summary>
		public void Dispose()
		{
			m_table = null;
			OnSaveVersion = null;
		}

		/// <summary>
		/// 初期化
		/// </summary>
		public IEnumerator Initialize()
		{
			m_versionFile = m_entryPoint.ToLocation(m_fileName);
			var path = m_versionFile.FullPath;
			ChipstarLog.Log_InitStorageDB(path);

			var isExist = File.Exists(path);
			if (!isExist)
			{
				//	なければ空データ
				m_table = new Table();
				ChipstarLog.Log_InitStorageDB_FirstCreate(path);
			}
			else
			{
				var bytes = File.ReadAllBytes(path);
				m_table = Load(bytes);
				ChipstarLog.Log_InitStorageDB_ReadLocalFile(m_table);
			}
			yield return null;
		}

		/// <summary>
		/// 取得
		/// </summary>
		public Hash128 GetVersion(ICachableBundle bundle)
		{
			var data = m_table.Get(bundle.Path);
			if (data == null)
			{
				return new Hash128();
			}
			return data.Version;
		}

		/// <summary>
		/// 
		/// </summary>
		protected Table Load(byte[] data)
		{
			return ParseLocalTable(data);
		}

		protected virtual Table ParseLocalTable(byte[] data)
		{
			var json = m_encoding.GetString(data);
			return JsonUtility.FromJson<Table>(json);
		}

		/// <summary>
		/// キャッシュ保持
		/// </summary>
		public bool HasStorage(ICachableBundle bundleData)
		{
			var data = m_table.Get(bundleData.Path);
			if (data == null)
			{
				return false;
			}
			// 破損チェック / 一旦サイズチェックだけ
			if (IsBreakFile( bundleData ))
			{
				return false;
			}
			// バージョン不一致
			if (!data.IsMatchVersion(bundleData))
			{
				ChipstarLog.Log_MissMatchVersion(bundleData.Path, data.Version.ToString(), bundleData.Hash.ToString());
				return false;
			}
			//
			return true;
		}
		public bool IsBreakFile( ICachableBundle bundleData )
		{
			var file = ToLocation(bundleData);
			if (!File.Exists(file.FullPath))
			{
				return false;
			}
			var info = new FileInfo(file.FullPath);
			var isBreak = bundleData.PreviewSize != info.Length;
			if (isBreak)
			{
				ChipstarLog.Log_MaybeFileBreak(info, bundleData.PreviewSize);
			}
			return isBreak;
		}

		/// <summary>
		/// キャッシュとバージョンの書き込み
		/// </summary>
		public virtual void Save(ICachableBundle data)
		{
			//	ファイルの書き込み
			if (OnSaveVersion?.Invoke(data) ?? false)
			{ 
				SaveVersion(data);
			}
		}

		/// <summary>
		/// 削除
		/// </summary>
		public virtual void Delete(ICachableBundle data)
		{
			DeleteBundle(data);
			RemoveVersion(data);
		}

		/// <summary>
		/// 保存
		/// </summary>
		public virtual void Apply()
		{
			var path = m_versionFile.FullPath;
			var dirPath = Path.GetDirectoryName(path);
			if (!Directory.Exists(dirPath))
			{
				Directory.CreateDirectory(dirPath);
			}
			var json = JsonUtility.ToJson(m_table, true);

			File.WriteAllText(path, json, m_encoding);
			ChipstarLog.Log_ApplyLocalSaveFile(path);
		}

		/// <summary>
		/// 場所の取得
		/// </summary>
		public IAccessLocation ToLocation(ICachableBundle data)
		{
			return m_entryPoint.ToLocation(data.Path);
		}

		/// <summary>
		/// キャッシュ済みデータの検索
		/// </summary>
		public IEnumerable<ICachableBundle> GetCachedList()
		{
			if (m_table.Count() <= 0)
			{
				return new ICachableBundle[0];
			}

			//	セーブデータにあるヤツは全部そう
			var cacheLiist = m_table
					.OfType<ICachableBundle>()
					.ToArray();
			return cacheLiist;
		}
		/// <summary>
		/// バージョンの保存
		/// </summary>
		private void SaveVersion(ICachableBundle data)
		{
			ChipstarLog.Log_SaveLocalVersion(data);
			//  ストレージにあるかどうか
			var storageData = m_table.Get(data.Path);
			if (storageData == null)
			{
				//  なければ追加書き込み
				m_table.Add(data);
				return;
			}
			//  あったらバージョン情報を上書き
			storageData.Apply(data);
		}

		/// <summary>
		/// アセットバンドルの削除
		/// </summary>
		private void DeleteBundle(ICachableBundle data)
		{
			var location = ToLocation(data);
			var path = location.FullPath;
			if (!File.Exists(path))
			{
				//	存在しないなら削除しない
				return;
			}
			ChipstarLog.Log_DeleteLocalBundle(data);
			File.Delete(path);
		}
		/// <summary>
		/// 保存バージョンを破棄
		/// </summary>
		private void RemoveVersion(ICachableBundle data)
		{
			ChipstarLog.Log_RemoveLocalVersion(data);
			var storageData = m_table.Get(data.Path);
			if (storageData == null)
			{
				//	保存されてないなら消さなくていい
				return;
			}
			m_table.Remove(storageData);
		}

		/// <summary>
		/// クリーンアップ
		/// 保存先を空にする
		/// </summary>
		public void CleanUp()
		{
			//	フォルダごと削除
			if (Directory.Exists(m_entryPoint.BasePath))
			{
				Directory.Delete(m_entryPoint.BasePath, true);
			}
			//	空のインスタンスで上書き
			m_table = new Table();
		}

		public override string ToString()
		{
			var builder = new StringBuilder();

			foreach (var item in m_table)
			{
				builder.AppendLine(item.ToString());
			}
			return builder.ToString();
		}
		/// <summary>
		/// 
		/// </summary>
		public IAccessPoint GetCacheStorage()
		{
			return m_entryPoint;
		}
	}
}
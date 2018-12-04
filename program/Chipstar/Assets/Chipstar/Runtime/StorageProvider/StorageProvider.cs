using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chipstar.Downloads
{
	public interface IStorageProvider : IDisposable
	{
		IEnumerator AllClear();
	}
	/// <summary>
	/// ストレージ管理
	/// </summary>
	public class StorageProvider<TRuntimeData> : IStorageProvider
		where TRuntimeData : IRuntimeBundleData<TRuntimeData>
	{
		//=================================
		//	プロパティ
		//=================================
		private ILoadDatabase<TRuntimeData>	LoadDatabase	{ get; set; }
		private IStorageDatabase			StorageDatabase	{ get; set; }

		//=================================
		//	関数
		//=================================

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public StorageProvider( 
			ILoadDatabase<TRuntimeData> assetDatabase,
			IStorageDatabase			storageDatabase 
			)
		{
			LoadDatabase    = assetDatabase;
			StorageDatabase	= storageDatabase;
		}

		/// <summary>
		/// 破棄処理
		/// </summary>
		public void Dispose()
		{
			LoadDatabase = null;
			StorageDatabase= null;
		}

		/// <summary>
		/// キャッシュクリア
		/// </summary>
		public IEnumerator AllClear()
		{
			var list = LoadDatabase.BundleList;
			foreach( var bundle in list )
			{
				StorageDatabase.Delete( bundle );
				yield return null;
			}
			StorageDatabase.Apply();
		}
	}
}

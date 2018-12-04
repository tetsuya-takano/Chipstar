using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chipstar.Downloads
{
	public interface ICacheProvider : IDisposable
	{
		IEnumerator AllClear();
	}
	/// <summary>
	/// キャッシュ管理
	/// </summary>
	public class CacheProvider<TRuntimeData> : ICacheProvider
		where TRuntimeData : IRuntimeBundleData<TRuntimeData>
	{
		//=================================
		//	プロパティ
		//=================================
		private ILoadDatabase<TRuntimeData>	LoadDatabase	{ get; set; }
		private ICacheDatabase				CacheDatabase	{ get; set; }

		//=================================
		//	関数
		//=================================

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public CacheProvider( 
			ILoadDatabase<TRuntimeData> assetDatabase,
			ICacheDatabase				cacheDatabase 
			)
		{
			LoadDatabase    = assetDatabase;
			CacheDatabase	= cacheDatabase;
		}

		/// <summary>
		/// 破棄処理
		/// </summary>
		public void Dispose()
		{
			LoadDatabase = null;
			CacheDatabase= null;
		}

		/// <summary>
		/// キャッシュクリア
		/// </summary>
		public IEnumerator AllClear()
		{
			var list = LoadDatabase.BundleList;
			foreach( var bundle in list )
			{
				CacheDatabase.Delete( bundle );
				yield return null;
			}
			CacheDatabase.Apply();
		}
	}
}

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

		public override string ToString()
		{
			var builder = new StringBuilder();

			foreach( var bundle in LoadDatabase.BundleList )
			{
				var hasStorage		= StorageDatabase.HasStorage( bundle );
				var localVersion	= StorageDatabase.GetVersion( bundle );
				var localCrc        = 0u;
				var storage			= StorageDatabase.ToLocation( bundle.Name );
				var remoteVersion   = bundle.Hash;
				var remoteCrc       = bundle.Crc;
				var server          = LoadDatabase.ToBundleLocation( bundle );

				builder
					.AppendLine()
					.AppendLine( bundle.Name )
					.AppendLine( "[Local]")
					.AppendLine( storage.AccessPath )
					.AppendLine( localVersion.ToString() )
					.AppendLine( localCrc.ToString() )
					.AppendLine()
					.AppendLine( "[Remote]")
					.AppendLine( server.AccessPath )
					.AppendLine( remoteVersion.ToString() )
					.AppendLine( remoteCrc.ToString() )
					.AppendLine()
					.AppendFormat( "Not Update:{0}", hasStorage )
					.AppendLine()
					.AppendLine("================================================="); 
			}
			return builder.ToString();
		}
	}
}

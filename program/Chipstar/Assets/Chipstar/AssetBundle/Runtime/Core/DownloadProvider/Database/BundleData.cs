using UnityEngine;
using System.Collections;
using System;
using System.Text;

namespace Chipstar.Downloads
{
	public interface IRuntimeBundleData<T>
		: IDisposable,
		ICachableBundle,
		IDeepRefCountable
		where T : IRuntimeBundleData<T>
	{
		string Name { get; }
		AssetData<T>[] Assets { get; }
		T[] Dependencies { get; }
		bool IsOnMemory { get; }
		bool IsScene { get; }
		long FileSize { get; }
		string[] Labels { get; }
		AssetBundleRequest LoadAsync<TAssetType>(string path) where TAssetType : UnityEngine.Object;
		void Unload();
		void OnMemory(AssetBundle bundle);

		void Set(IBundleBuildData data);
		void Set(AssetData<T>[] assets);
		void Set(T[] dependencies);
	}

	public class AssetData<T> : IDisposable,
		IRefCountable
		where T : IRuntimeBundleData<T>
	{
		public string Path { get; private set; }
		public string Guid { get; private set; }
		public T BundleData { get; private set; }
		bool IRefCountable.IsFree => BundleData?.IsFree ?? false;
		int IRefCountable.RefCount => BundleData?.RefCount ?? 0;
		public AssetData(IAssetBuildData data)
		{
			Apply(data.Path, data.Guid);
		}

		public void Dispose()
		{
			Path = string.Empty;
			Guid = string.Empty;
			BundleData = default(T);
		}
		public void Apply(string path, string guid)
		{
			Path = path;
			Guid = guid;
		}
		public void Connect(T data)
		{
			BundleData = data;
		}
		internal AssetBundleRequest LoadAsync<TAssetType>() where TAssetType : UnityEngine.Object
		{
			return BundleData.LoadAsync<TAssetType>(Path);
		}
		public void AddRef() => BundleData?.AddDeepRef();
		public void ReleaseRef() => BundleData?.ReleaseDeepRef();
		void IRefCountable.ClearRef() => BundleData?.ClearRef();
	}

	public abstract class BundleData<T>
		: IRuntimeBundleData<T>
			where T : IRuntimeBundleData<T>
	{
		//========================================
		//  プロパティ
		//========================================

		public string Name { get; private set; }
		public AssetData<T>[] Assets { get; private set; }
		public T[] Dependencies { get; private set; }
		public bool IsOnMemory { get { return Bundle; } }
		public bool IsScene { get { return IsOnMemory ? Bundle.isStreamedSceneAssetBundle : false; } }
		public long FileSize { get; private set; }
		public bool IsFree { get { return RefCount <= 0; } }
		public string[] Labels { get; private set; }
		protected AssetBundle Bundle { get; set; }
		public int RefCount { get; private set; }
		public Hash128 Hash { get; private set; }
		public uint Crc { get; private set; }
		string ICachableBundle.Path { get { return Name; } }
		long ICachableBundle.PreviewSize { get { return FileSize; } }
		//========================================
		//  関数
		//========================================

		public void Dispose()
		{
			ClearRef();
			Unload();
			Assets = new AssetData<T>[0];
			Dependencies = new T[0];

		}

		public void Set(IBundleBuildData data)
		{
			Name = data.ABName;
			Hash = Hash128.Parse(data.Hash);
			Crc = data.Crc;
			FileSize = data.FileSize;
			Labels = data.Labels;
		}

		public void Set(AssetData<T>[] assets)
		{
			Assets = assets;
		}

		public void Set(T[] dependencies)
		{
			Dependencies = dependencies;
		}

		/// <summary>
		/// アセットバンドル保持
		/// </summary>
		public void OnMemory(AssetBundle bundle)
		{
			if (bundle == null)
			{
				Debug.LogAssertionFormat("Load Bundle Is Null :{0}", Name);
			}
			Bundle = bundle;
		}

		/// <summary>
		/// 読み込み
		/// </summary>
		public AssetBundleRequest LoadAsync<TAssetType>(string path) where TAssetType : UnityEngine.Object
		{
			if (Bundle == null)
			{
				return null;
			}
			return Bundle.LoadAssetAsync<TAssetType>(path);
		}

		/// <summary>
		/// 破棄
		/// </summary>
		public void Unload()
		{
			if (!IsFree)
			{
				ChipstarLog.Log_Unload_Error(this);
				return;
			}
			if (Bundle)
			{
				ChipstarLog.Log_Unload_Bundle( this );
				Bundle.Unload(true);
			}
			Bundle = null;
		}

		/// <summary>
		/// 参照カウンタ加算
		/// </summary>
		public void AddRef()
		{
			RefCount++;
		}

		/// <summary>
		/// 参照カウンタ減算
		/// </summary>
		public void ReleaseRef()
		{
			RefCount = Mathf.Max(0, RefCount - 1);
		}

		/// <summary>
		/// 参照カウンタ破棄
		/// </summary>
		public void ClearRef()
		{
			RefCount = 0;
		}

		public void AddDeepRef()
		{
			AddRef();
			foreach( var d in Dependencies )
			{
				d.AddRef();
			}
		}
		public void ReleaseDeepRef()
		{
			ReleaseRef();
			foreach (var d in Dependencies)
			{
				d.ReleaseRef();
			}
		}
		public override string ToString()
		{
			var builder = new StringBuilder();

			builder.AppendLine(Name);
			builder.AppendLine("Hash : " + Hash.ToString());
			builder.AppendLine("Crc : " + Crc.ToString());
			builder.AppendLine("Ref : " + RefCount.ToString());
			builder.AppendLine("OnMemory : " + IsOnMemory.ToString());
			builder.AppendLine("FileSize : " + FileSize.ToString());
			builder.AppendLine("[Dependencies]");
			foreach (var d in Dependencies)
			{
				builder.Append("   -").AppendLine(d.Name);
			}
			builder.AppendLine("[Label]");
			foreach (var l in Labels)
			{
				builder.Append("   -").AppendLine(l);
			}
			return builder.ToString();
		}
	}
}
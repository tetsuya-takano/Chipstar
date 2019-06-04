using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Chipstar.Downloads
{
	public interface IAssetVersion
	{
		string Hash { get; }
	}
	/// <summary>
	/// バージョン値管理クラス
	/// </summary>
	public sealed class AssetVersion : IAssetVersion
	{
		//===============================
		//	プロパティ
		//===============================
		public string Hash { get; private set; } = string.Empty;

		//===============================
		//	関数
		//===============================
		public AssetVersion(string hash )
		{
			Hash = hash;
		}

		public override string ToString()
		{
			return $"Hash:{Hash}";
		}
	}
}
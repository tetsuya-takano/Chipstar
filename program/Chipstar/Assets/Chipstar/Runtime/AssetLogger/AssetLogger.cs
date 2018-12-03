using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chipstar.Downloads
{
	public interface IAssetLogger : IDisposable
	{
		void LoadLevelAdditive( string path );
		void LoadLevel( string path );
		void LoadAsset<T>( string path ) where T : UnityEngine.Object;
	}
	/// <summary>
	/// ログを出さない
	/// </summary>
	public sealed class NothingLogger : IAssetLogger
	{
		public static NothingLogger Default = new NothingLogger();
		public void Dispose() { }

		public void LoadAsset<T>( string path ) where T : UnityEngine.Object { }
		public void LoadLevel( string path ) { }
		public void LoadLevelAdditive( string path ) { }
	}
	/// <summary>
	/// ロガー
	/// </summary>
	public class AssetLogger : IAssetLogger
	{
		/// <summary>
		/// 破棄処理
		/// </summary>
		public void Dispose()
		{
		}

		/// <summary>
		/// 読み込み
		/// </summary>
		public void LoadAsset<T>( string path ) where T : UnityEngine.Object
		{
			Debug.LogFormat( "[Asset Loader] LoadAsset {0} ({1})", path, typeof( T ) );
		}

		/// <summary>
		/// 遷移
		/// </summary>
		public void LoadLevel( string path )
		{
			Debug.Log( "[Asset Loader] LoadLevel" + path );
		}

		/// <summary>
		/// 加算
		/// </summary>
		public void LoadLevelAdditive( string path )
		{
			Debug.Log( "[Asset Loader] LoadLevel Additive" + path );
		}
	}
}
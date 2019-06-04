using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Chipstar.Builder
{
	/// <summary>
	/// マニフェストファイルのUtility
	/// </summary>
	public static class ABManifestUtility
	{
		/// <summary>
		/// ハッシュ直取得
		/// </summary>
		public static Hash128 TryGetHash( this AssetBundleManifest self, string abName )
		{
			if( !self)
			{
				return new Hash128();
			}
			return self.GetAssetBundleHash( abName );
		}
		public static string TryGetHashString( this AssetBundleManifest self, string abName )
		{
			var hash = self.TryGetHash( abName );
			if( hash.isValid )
			{
				return hash.ToString();
			}
			return string.Empty;
		}

		/// <summary>
		/// 依存直取得
		/// </summary>
		public static string[] TryGetDependencies( this AssetBundleManifest self, string abName )
		{
			if( !self )
			{
				return new string[ 0 ];
			}
			return self.GetAllDependencies( abName );
		}

	}
	/// <summary>
	/// ファイルUtility
	/// </summary>
	public static class FsUtillity
	{
		/// <summary>
		/// Crc取得
		/// </summary>
		public static uint TryGetCrc( string path )
		{
			var crc = 0u;
			if( !BuildPipeline.GetCRCForAssetBundle( path, out crc ) )
			{
				return crc;
			}
			return crc;
		}

		/// <summary>
		/// ファイルサイズ取得
		/// </summary>
		public static long TryGetFileSize( string path )
		{
			if( !File.Exists( path ) )
			{
				return 00;
			}
			var info = new FileInfo( path );
			return info.Length;
		}
	}

	/// <summary>
	/// 進捗ダイアログスコープ
	/// </summary>
	public sealed class ProgressDialogScope : IDisposable
	{
		//==============================
		//	変数
		//==============================
		private string	m_title = "";
		private int     m_count = 0;

		//==============================
		//	関数
		//==============================

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ProgressDialogScope( string title, int count )
		{
			m_title = title;
			m_count = count;
		}

		/// <summary>
		/// 表示
		/// </summary>
		public void Show( string message, int current )
		{
			var progress = Mathf.InverseLerp( 0, m_count, current );
			EditorUtility.DisplayProgressBar( m_title, message, progress );
		}
		/// <summary>
		/// 破棄
		/// </summary>
		public void Dispose()
		{
			EditorUtility.ClearProgressBar();
		}
	}
}
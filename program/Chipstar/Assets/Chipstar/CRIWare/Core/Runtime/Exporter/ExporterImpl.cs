using System.Collections;
using System.Collections.Generic;
using Chipstar.Downloads;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;

namespace Chipstar.Downloads.CriWare
{
	/// <summary>
	/// Android用
	/// </summary>
	public sealed class AndroidExporter : LocalFileExporter
	{
		//==========================================
		//	変数
		//==========================================
		private IAccessPoint m_exportDir = null;
		private IAccessPoint m_sourceDir = null;

		//==========================================
		//	関数
		//==========================================

		public AndroidExporter( IAccessPoint exportDir )
		{
			m_sourceDir = new AccessPoint( Application.streamingAssetsPath );
			m_exportDir = exportDir;
		}

		/// <summary>
		/// 出力
		/// </summary>
		public override IEnumerator Run( string relativePath )
		{
			//	パス
			var sourcePath = m_sourceDir.ToLocation( relativePath );
			var destPath   = m_exportDir.ToLocation( relativePath );
			if( File.Exists( destPath.FullPath ) )
			{
				yield break;
			}

			//	リクエスト
			yield return CopyFile( sourcePath.FullPath, destPath.FullPath );
		}
	}

	/// <summary>
	/// 何もしない
	/// </summary>
	public sealed class EmptyExporter : LocalFileExporter
	{
		public EmptyExporter() { }
	}
}
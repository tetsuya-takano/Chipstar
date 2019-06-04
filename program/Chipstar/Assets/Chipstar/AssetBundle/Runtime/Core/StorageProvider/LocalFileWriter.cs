using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Chipstar.Downloads
{
	public interface ILocalFileWriter : IDisposable
	{
		bool Write( IAccessLocation location, byte[] contents );
	}
	/// <summary>
	/// ローカルにファイルを保存するための機構
	/// </summary>
	public sealed class LocalFileWriter : ILocalFileWriter
	{
		//======================================
		//	const
		//======================================
		private const string WRITE_TEMP_FILE_NAME  = "{0}.tmp";
		private const string DELETE_TEMP_FILE_NAME = "{0}.del";
		private const float  WRITE_BUFFER_TIME     = 1f;

		//======================================
		//	変数
		//======================================
		private float m_timeCount = 0;

		//======================================
		//	関数
		//======================================

		public void Dispose() { }

		/// <summary>
		/// 書き込み
		/// </summary>
		public bool Write( IAccessLocation location, byte[] contents )
		{
			if( contents == null || contents.Length <= 0 )
			{
				return false;
			}
			var filePath = location.FullPath;
			var dirPath  = Path.GetDirectoryName( filePath );
			if( !Directory.Exists( dirPath ) )
			{
				Directory.CreateDirectory( dirPath );
			}

			//	既存ファイルの削除
			if( !DeleteOld( filePath ))
			{
				return false;
			}
			//	一時書き込み
			if( !WriteTemp( filePath, contents ))
			{
				return false;
			}
			return true;
		}

		/// <summary>
		/// 一時書き込み
		/// </summary>
		private bool WriteTemp( string path, byte[] datas )
		{
			//	tmpとして 一時書き込み
			var tmpPath = string.Format( WRITE_TEMP_FILE_NAME, path );
			if( File.Exists( tmpPath ) )
			{
				File.Delete( tmpPath );
			}
			if( !IsCheckFileDelete( tmpPath ))
			{
				return false;
			}
			File.WriteAllBytes( tmpPath, datas );
			//	リネーム
			File.Move( tmpPath, path );

			return true;
		}

		/// <summary>
		/// 古いファイルを削除する
		/// </summary>
		private bool DeleteOld( string path )
		{
			var delTmpPath = string.Format( DELETE_TEMP_FILE_NAME, path );

			if( !File.Exists( path ))
			{
				return true;
			}
			//	一旦退避する
			File.Move( path, delTmpPath );
			//	削除
			if( File.Exists( delTmpPath ) )
			{
				File.Delete( delTmpPath );
			}

			if( !IsCheckFileDelete( delTmpPath ) )
			{
				return false;
			}
			return true;
		}

		private bool IsCheckFileDelete( string targetPath )
		{
			//	開始時間
			var startTime = Time.realtimeSinceStartup;
			//	削除を待つ
			while( true )
			{
				var currentTime = Time.realtimeSinceStartup;
				//	削除されたら終了
				if( !File.Exists( targetPath ) )
				{
					break;
				}
				if( (currentTime - startTime) > WRITE_BUFFER_TIME )
				{
					//	１秒以上かかったら削除に失敗とみなす
					return false;
				}
			}
			return true;
		}
	}
}
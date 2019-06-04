using Chipstar.Downloads;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using System;

namespace Chipstar.Downloads.CriWare
{
	public interface ILocalFileExporter
	{
		IEnumerator Run( string relativePath );
	}
	/// <summary>
	/// 内包ファイルをドコかに出力する機能
	/// </summary>
	public abstract class LocalFileExporter : ILocalFileExporter
	{
		//==================================
		//	変数
		//==================================

		//==================================
		//	関数
		//==================================

		/// <summary>
		/// 生成
		/// </summary>
		public static LocalFileExporter Create( IAccessPoint includeDir )
		{
#if DISABLE_ANDROID_OBB
			return new EmptyExporter();
#else
	#if !UNITY_EDITOR && UNITY_ANDROID
				return new AndroidExporter( includeDir );
	#else
				return new EmptyExporter();
	#endif
#endif
		}
		public virtual IEnumerator Run( string relativePath )
		{
			yield break;
		}
		/// <summary>
		/// 取得
		/// </summary>
		protected virtual IEnumerator CopyFile( string sourcePath, string targetPath )
		{
			var www     = UnityWebRequest.Get( sourcePath );
			//	DLハンドラ
			var handler = new DownloadHandlerFile( targetPath );
			handler.removeFileOnAbort = true;
			www.downloadHandler = handler;

			//	開始
			var request = www.SendWebRequest();
			while( !request.isDone )
			{
				var w = request.webRequest;
				if( w.isDone )
				{
					break;
				}
				if( w.isHttpError || w.isNetworkError )
				{
					throw new FileLoadException( w.error );
				}
				yield return null;
			}
			yield return request;
		}

	}
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Chipstar.Downloads
{
	public interface IManifestLoader
	{
		IEnumerator Login( IAccessPoint server, IAssetVersion version );
		FileVersionManifest Parse();
	}
	/// <summary>
	/// ファイルバージョンManifestを取得するモノ
	/// </summary>
	public class ManifestLoader : IManifestLoader
	{
		//=================================
		//	変数
		//=================================
		private string m_fileName = string.Empty;
		private string m_contents = null;
		//=================================
		//	関数
		//=================================

		public ManifestLoader(string fileName)
		{
			m_fileName = fileName;
		}
		/// 
		/// </summary>
		public IEnumerator Login( IAccessPoint server, IAssetVersion version )
		{
			//	manifestのパス
			//	s3 : xxxx // server-version / fileName
			var manifestLocation = server
									.ToAppend( version.Hash )
									.ToLocation( m_fileName );
			ChipstarLog.Log_RequestVersionManifest( manifestLocation );
			//	とりあえず素の機能で落とす
			var www = UnityWebRequest.Get( manifestLocation.FullPath );
			www.SendWebRequest();

			while( !www.isDone )
			{
				yield return www;
				if( www.isHttpError || www.isNetworkError )
				{
					break;
				}
			}
			if (!www.isNetworkError && !www.isHttpError)
			{
				m_contents = www.downloadHandler.text;
			}
			ChipstarLog.Log_DLVersionManifest( m_contents );
		}

		/// <summary>
		/// 
		/// </summary>
		public FileVersionManifest Parse()
		{
			var obj = FileVersionManifest.FromJson( m_contents );
			if (obj == null)
			{
				return new FileVersionManifest();
			}
			return obj;
		}
	}
}
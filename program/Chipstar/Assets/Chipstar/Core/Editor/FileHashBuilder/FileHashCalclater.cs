using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using UnityEngine;

namespace Chipstar.Builder
{
	public interface IFileHashCalclater : IDisposable
	{
		string Calclate( FileInfo info );
	}
	/// <summary>
	/// ハッシュ値計算用のクラス
	/// </summary>
	public class FileHashCalclater<T> 
		: IFileHashCalclater
		where T : HashAlgorithm
	{
		//=============================
		//	コンストラクタ
		//=============================
		private T m_hashObj = default(T);

		//=============================
		//	関数
		//=============================
		public FileHashCalclater( T hashObj )
		{
			m_hashObj = hashObj;
		}
		/// <summary>
		/// 計算
		/// </summary>
		public string Calclate( FileInfo info )
		{
			using( var fs = info.OpenRead() )
			{
				return DoCalclate( fs );
			}
		}

		protected virtual string DoCalclate( FileStream fs )
		{
			var bytes   = m_hashObj.ComputeHash( fs );
			var hash    = BitConverter.ToString( bytes );

			return hash.Replace( "-", string.Empty ).ToLower();
		}

		/// <summary>
		/// 破棄処理
		/// </summary>
		public void Dispose()
		{
			m_hashObj.DisposeIfNotNull();
		}
	}
}
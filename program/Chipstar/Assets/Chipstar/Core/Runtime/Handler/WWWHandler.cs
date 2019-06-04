using UnityEngine;
using System.Collections;
using System;
using System.IO;

namespace Chipstar.Downloads
{
	/// <summary>
	/// WWWでダウンロードするクラス
	/// </summary>
	public static partial class WWWDL
	{
		//==================================
		//  各通信用ジョブの作成
		//==================================
		public static ILoadJob<string> GetTextFile(IAccessLocation location)
		{
			return new WWWDLJob<string>(location, new WWWDL.TextDL());
		}
		public static ILoadJob<byte[]> GetBinaryFile(IAccessLocation location)
		{
			return new WWWDLJob<byte[]>(location, new WWWDL.BytesDL());
		}
		public static ILoadJob<AssetBundle> GetAssetBundle(IAccessLocation location)
		{
			return new WWWDLJob<AssetBundle>(location, new WWWDL.AssetBundleDL());
		}
		public static ILoadJob<Empty> GetFileDL(IAccessLocation location, IAccessLocation local)
		{
			return new WWWDLJob<Empty>(location, new WWWDL.FileDL( local ));
		}

		//==================================
		//  各データ取得用ハンドラ定義
		//==================================
		/// <summary>
		/// WWWで取る機能
		/// </summary>
		public abstract class WWWHandler<T> : DLHandler<WWW, T> { }

		/// <summary>
		/// テキスト
		/// </summary>
		public sealed class TextDL : WWWHandler<string>
		{
			protected override string DoComplete(WWW source)
			{
				return source.text;
			}
		}
		/// <summary>
		/// 生Bytes
		/// </summary>
		public sealed class BytesDL : WWWHandler<byte[]>
		{
			protected override byte[] DoComplete(WWW source)
			{
				return source.bytes;
			}
		}

		/// <summary>
		/// バンドル
		/// </summary>
		public sealed class AssetBundleDL : WWWHandler<AssetBundle>
		{
			protected override AssetBundle DoComplete(WWW source)
			{
				return source.assetBundle;
			}
		}

		public sealed class FileDL : WWWHandler<Empty>
		{
			private IAccessLocation m_local;
			public FileDL(IAccessLocation local)
			{
				m_local = local;
			}
			protected override Empty DoComplete(WWW source)
			{
				var directory = Path.GetDirectoryName( m_local.FullPath );
				if(!Directory.Exists(directory))
				{
					Directory.CreateDirectory(directory);
				}
				// tmp書き込み
				var tmpName = m_local.FullPath + ".tmp";
				if (File.Exists(tmpName))
				{
					File.Delete( tmpName );
				}
				if (File.Exists(m_local.FullPath))
				{
					File.Delete(m_local.FullPath);
				}

				var datas = source.bytes;
				// tmp書き込み
				File.WriteAllBytes(tmpName, datas);
				// Rename
				File.Move(tmpName, m_local.FullPath);

				return Empty.Default;
			}
		}
	}
}
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using UnityEngine;

namespace Chipstar.Downloads
{
	/// <summary>
	/// Jsonそのままで保存
	/// </summary>
	public class CompressTextWriter<T> : DatabaseWriter<T>
	{
		private Encoding Encoding = null;
		public CompressTextWriter(Encoding encode) : base() { Encoding = encode; }

		protected override byte[] Compress(T obj)
		{
			var contents = JsonUtility.ToJson(obj, true);
			var rawDatas = Encoding.GetBytes(contents);
			using (var memStream = new MemoryStream())
			{
				using (var gzipStream = new GZipStream(memStream, CompressionMode.Compress))
				{
					gzipStream.Write(rawDatas, 0, rawDatas.Length);
				}
				return memStream.ToArray();
			}
		}
	}
}

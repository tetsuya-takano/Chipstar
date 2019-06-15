using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using UnityEngine;
namespace Chipstar.Downloads
{
	/// <summary>
	/// 
	/// </summary>
	public class CompressedJsonParser<T> : JsonDatabaseParser<T> where T : new()
    {
		public CompressedJsonParser(Encoding encode) : base(encode)
		{
		}

		protected override byte[] Decompress(byte[] datas)
		{
			var buffer = new byte[ 1024 * 4 ];
			using (var readStream = new MemoryStream(datas))
			using (var gzipStream = new GZipStream(readStream, CompressionMode.Decompress))
			using (var writeStream = new MemoryStream())
			{
				int readSize = 0;
				do
				{
					readSize = gzipStream.Read(buffer, 0, buffer.Length);
					if (readSize <= 0)
					{
						break;
					}
					writeStream.Write(buffer, 0, readSize);
				}
				while (readSize > 0);

				return writeStream.ToArray();
			}
		}
	}
}
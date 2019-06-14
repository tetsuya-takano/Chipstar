using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace Chipstar.Builder
{
	public interface IManifestWriter
	{
		void Write(string path, object tableContents );
	}
	public abstract class ManifestWriter : IManifestWriter
	{
		//====================================
		//	変数
		//====================================
		//====================================
		//	関数
		//====================================
		public ManifestWriter() { }
		public void Write(string path, object tableContents )
		{
			var contents = Compress( tableContents );
			File.WriteAllBytes(path, contents );
		}

		protected abstract byte[] Compress(object obj);
	}
}
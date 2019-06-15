using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace Chipstar.Downloads
{
	public interface IDatabaseWriter<T>
	{
		void Write(string path, T tableContents );
	}
	public abstract class DatabaseWriter<T> : IDatabaseWriter<T>
	{
		//====================================
		//	変数
		//====================================
		//====================================
		//	関数
		//====================================
		public DatabaseWriter() { }
		public void Write(string path, T tableContents )
		{
			var contents = Compress( tableContents );
			File.WriteAllBytes(path, contents );
		}

		protected abstract byte[] Compress(T obj);
	}
}
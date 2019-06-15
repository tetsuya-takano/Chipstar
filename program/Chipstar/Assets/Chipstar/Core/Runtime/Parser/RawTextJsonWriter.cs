using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Chipstar.Downloads
{
	/// <summary>
	/// Jsonそのままで保存
	/// </summary>
	public class RawTextJsonWriter<T> : DatabaseWriter<T>
	{
		private Encoding Encoding = null;
		public RawTextJsonWriter(Encoding encode) { Encoding = encode; }

		protected override byte[] Compress(T obj)
		{
			var contents = JsonUtility.ToJson(obj, true);

			return Encoding.GetBytes( contents );
		}
	}
}
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Chipstar.Builder
{
	/// <summary>
	/// Jsonそのままで保存
	/// </summary>
	public class RawTextJsonWriter : ManifestWriter
	{
		private Encoding Encoding = null;
		public RawTextJsonWriter(Encoding encode) { Encoding = encode; }

		protected override byte[] Compress(object obj)
		{
			var contents = JsonUtility.ToJson(obj, true);

			return Encoding.GetBytes( contents );
		}
	}
}
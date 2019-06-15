using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
namespace Chipstar.Downloads
{
	public interface IDatabaseParser<T>
	{
		T Parse(byte[] datas);
	}

	/// <summary>
	/// Jsonで扱う
	/// </summary>
	public abstract class JsonDatabaseParser<T> : IDatabaseParser<T>
	{
		//=============================
		//	変数
		//=============================
		protected readonly Encoding Encoding = System.Text.Encoding.UTF8;

		//=============================
		//	関数
		//=============================
		public JsonDatabaseParser(Encoding encode)
		{
			Encoding = encode;
		}

		public T Parse(byte[] datas)
		{
			var json = Encoding.GetString(Decompress(datas));
			return JsonUtility.FromJson<T>(json);
		}
		protected abstract byte[] Decompress(byte[] datas);
	}

	
}
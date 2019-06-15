using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
namespace Chipstar.Downloads
{
	public interface IDatabaseParser<T> where T : new()
	{
		T Parse(byte[] datas);
        T CreateEmpty();
    }

	/// <summary>
	/// Jsonで扱う
	/// </summary>
	public abstract class JsonDatabaseParser<T> : IDatabaseParser<T>
        where T : new()
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

        public T CreateEmpty()
        {
            return new T();
        }

        public T Parse(byte[] datas)
		{
			var json = Encoding.GetString(Decompress(datas));
			return JsonUtility.FromJson<T>(json);
		}
		protected abstract byte[] Decompress(byte[] datas);
	}

	
}
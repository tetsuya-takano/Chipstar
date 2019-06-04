using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chipstar
{
	public static class ChipstarUtils
	{
		/// <summary>
		/// 区切り文字の変換
		/// \\ -> /
		/// </summary>
		public static string ToConvertDelimiter( this string self )
		{
			return self.Replace("\\", "/");
		}

		public static void DisposeIfNotNull( this IDisposable self )
		{
			if (self == null)
			{
				return;
			}
			self.Dispose();
		}
	}
}
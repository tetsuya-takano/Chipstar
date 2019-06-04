using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Chipstar.Downloads
{
	public interface ICachableBundle
	{
		long PreviewSize { get; }
		string Path { get; }
		Hash128	Hash { get; }
		uint	Crc  { get; }
	}

	/// <summary>
	/// 
	/// </summary>
	public static class ICachableBundleExtensions
	{
		/// <summary>
		/// 
		/// </summary>
		public static string ToCacheDataStr( this ICachableBundle self )
		{
			return string.Format( "{0}[{1}][{2}]", self.Path, self.Hash, self.Crc );
		}
	}
}

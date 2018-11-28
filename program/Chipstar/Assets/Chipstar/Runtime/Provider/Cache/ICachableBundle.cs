using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Chipstar.Downloads
{
	public interface ICachableBundle
	{
		string	Name { get; }
		Hash128	Hash { get; }
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
			return string.Format( "{0} == {1}", self.Name, self.Hash );
		}
	}
}

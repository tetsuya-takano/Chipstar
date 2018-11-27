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
}

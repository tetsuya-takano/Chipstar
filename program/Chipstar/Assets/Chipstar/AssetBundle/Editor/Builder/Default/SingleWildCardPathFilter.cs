using Chipstar.Builder;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
namespace Chipstar.Builder
{
    /// <summary>
    /// 1個のワイルドカードを許容する
    /// </summary>
    public class SingleWildCardPathFilter : ABPathFilter
    {
        private Regex m_pattern = null;
        public SingleWildCardPathFilter(string pattern) : base(pattern)
        {
        }

		protected override bool DoMatch(string rootFolder, string path)
		{
			if( m_pattern == null )
			{
				var p = Path.Combine(rootFolder, Pattern)
						.ToConvertDelimiter()
						.Replace("*", "(.*?)");
				m_pattern = new Regex( p );
			}
			return m_pattern.IsMatch( path );
        }
    }
}
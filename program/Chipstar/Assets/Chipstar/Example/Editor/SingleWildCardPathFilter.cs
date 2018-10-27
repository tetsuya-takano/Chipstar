using Chipstar.Builder;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
namespace Chipstar.Example
{
    /// <summary>
    /// 1個のワイルドカードを許容する
    /// </summary>
    public class SingleWildCardPathFilter : ABPathFilter
    {
        private Regex m_pattern = null;
        public SingleWildCardPathFilter(string pattern) : base(pattern)
        {
            m_pattern = new Regex( pattern.Replace( "*", "(.*?)" ) );
        }

        protected override bool DoMatch( string path )
        {
            return m_pattern.IsMatch( path );
        }
    }
}
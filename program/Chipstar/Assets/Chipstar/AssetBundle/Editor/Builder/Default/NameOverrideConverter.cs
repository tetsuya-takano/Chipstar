using UnityEngine;
using UnityEditor;
using Chipstar.Builder;
using System.Text.RegularExpressions;

namespace Chipstar.Builder
{
    public class NameOverrideConverter : ABNameConverter
    {
        protected string PackName { get; set; }
        private Regex m_regex = null;
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public NameOverrideConverter( 
            string packName, string extension
        ) : base(extension)
        {
            PackName = packName;
            m_regex  = new Regex( PackName.Replace( "*", "(.*?)" ), RegexOptions.IgnoreCase );
        }

        protected override string DoConvert(string assetPath)
        {
            //  指定なし
            if (string.IsNullOrEmpty(PackName))
            {
                return assetPath;
            }
            if( PackName.Contains( "*" ) )
            {
                var match = m_regex.Match( assetPath );   
                if( !match.Success )
                {
                    return assetPath;
                }
                var uniqueStr = match.Groups[1].Value;
                return PackName.Replace( "*", uniqueStr );

            }
            return PackName;
        }
    }
}
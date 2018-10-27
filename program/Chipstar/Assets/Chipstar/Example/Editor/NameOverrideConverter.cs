using UnityEngine;
using UnityEditor;
using Chipstar.Builder;

namespace Chipstar.Example
{
    public class NameOverrideConverter : ABNameConverter
    {
        protected string PackName { get; set; }
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public NameOverrideConverter( 
            string packName, string extension
        ) : base(extension)
        {
            PackName = packName;
        }

        protected override string DoConvert(string assetPath)
        {
            //  指定なし
            if (string.IsNullOrEmpty(PackName))
            {
                return assetPath;
            }
            return PackName;
        }
    }
}
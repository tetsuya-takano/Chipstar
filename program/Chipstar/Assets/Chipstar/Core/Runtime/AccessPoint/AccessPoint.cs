using UnityEngine;
using System.Collections;
using System.IO;

namespace Chipstar.Downloads
{
    public interface IAccessPoint
    {
        string BasePath { get; }
		IAccessPoint ToAppend(string relativePath);
		IAccessLocation ToLocation(string relativePath);
	}
	/// <summary>
	/// アクセスポイント用クラス
	/// </summary>
    public class AccessPoint : IAccessPoint
    {
        //==================================
        //  プロパティ
        //==================================
        public string BasePath { get; private set; }
        //==================================
        //  関数
        //==================================

        public AccessPoint( string path )
        {
            BasePath = path;
        }

		/// <summary>
		/// 所在の取得
		/// </summary>
        public IAccessLocation ToLocation( string relativePath )
        {
			return new UrlLocation( relativePath, ToAccessPath( relativePath ) );
		}
		/// <summary>
		/// 結合
		/// </summary>
		public IAccessPoint ToAppend( string relativePath )
		{
			var location = ToLocation( relativePath );
			return new AccessPoint( location.FullPath );
		}

        protected virtual string ToAccessPath( string path )
        {
			if (string.IsNullOrEmpty(path))
			{
				return BasePath.ToConvertDelimiter();
			}
            return Path.Combine(BasePath, path).ToConvertDelimiter();
        }

		public override string ToString()
		{
			return $"{GetType().Name}:{BasePath}";
		}
	}
}
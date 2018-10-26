using System;

namespace Chipstar.Builder
{
	/// <summary>
	/// 名前を変換する
	/// </summary>
	public interface IABNameConverter 
	{
		string		Convert( string assetPath );
	}

    /// <summary>
    /// 
    /// </summary>
    public class ABNameConverter : IABNameConverter
    {
        public static readonly ABNameConverter Empty = new ABNameConverter( ".ab" );

        protected string Extension { get; set; }

        public ABNameConverter( string extension )
        {
            Extension = extension;
        }

        public virtual string Convert( string assetPath )
        {
            return DoConvert( assetPath ) + Extension;
        }

        protected virtual string DoConvert( string assetPath )
        {
            return assetPath;
        }
    }
}
namespace Chipstar.Builder
{
	/// <summary>
	/// パスを絞り込むインターフェース
	/// </summary>
	public interface IABPathFilter
	{
		bool IsMatch( string path );
	}
    public class ABPathFilter : IABPathFilter
    {
        //====================================
        //  変数
        //====================================
        
        //====================================
        //  プロパティ
        //====================================

        protected string Pattern { get; set; }

        //====================================
        //  関数
        //====================================

        public ABPathFilter(string pattern)
        {
            Pattern = pattern;
        }

        public virtual bool IsMatch( string path )
        {
            return DoMatch( path );
        }
        protected virtual bool DoMatch( string path )
        {
            return path.Contains( Pattern );
        }
    }
}
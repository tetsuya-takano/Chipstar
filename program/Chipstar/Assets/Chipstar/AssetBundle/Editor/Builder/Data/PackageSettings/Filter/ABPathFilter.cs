namespace Chipstar.Builder
{
	/// <summary>
	/// パスを絞り込むインターフェース
	/// </summary>
	public interface IABPathFilter
	{
		bool IsMatch( string rootFolder, string path );
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

        public virtual bool IsMatch( string rootFolder, string path )
        {
			return DoMatch(rootFolder, path);
		}
        protected virtual bool DoMatch(string rootFolder, string path )
        {
            return path.Contains( Pattern );
        }
    }
}
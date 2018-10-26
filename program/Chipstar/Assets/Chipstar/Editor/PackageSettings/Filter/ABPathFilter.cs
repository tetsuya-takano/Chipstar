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
        public static readonly ABPathFilter Empty = new ABPathFilter();

        public virtual bool IsMatch( string path )
        {
            return DoMatch( path );
        }
        protected virtual bool DoMatch( string path ) { return true; }
    }
}
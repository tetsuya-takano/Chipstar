namespace Chipstar.Downloads
{
    /// <summary>
    /// URLで場所を知らせる
    /// </summary>
    public sealed class UrlLocation : DLLocation
    {
		//=================================
		//  プロパティ
		//=================================
		private string Key { get; set; }
		private string Url { get; set; }

		public override string AccessKey	{ get { return Key; } }
		public override string FullPath		{ get { return Url; } }

		//=================================
		//  関数
		//=================================

		public UrlLocation( string key, string url )
		{
			Key = key;
            Url = url;
        }
    }
}
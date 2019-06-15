using System.Text;
namespace Chipstar.Downloads
{
	/// <summary>
	/// 生テキストで取得
	/// </summary>
	public sealed class RawTextJsonParser<T> : JsonDatabaseParser<T> where T : new()
    {
		public RawTextJsonParser(Encoding encode) : base(encode) { }
		protected override byte[] Decompress(byte[] datas) { return datas; }
	}
}
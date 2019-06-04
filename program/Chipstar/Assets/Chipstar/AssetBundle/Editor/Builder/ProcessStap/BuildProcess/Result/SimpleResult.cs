using UnityEngine;
using UnityEditor;
namespace Chipstar.Builder
{

    public class SimpleResult : IABBuildResult
    {
        public bool IsSuccess { get; set; }

        public SimpleResult( bool success )
        {
            IsSuccess = success;
        }
    }

    public class ABBuildResult : IABBuildResult
    {
        public AssetBundleManifest  Manifest  { get; private set; }
        public bool                 IsSuccess { get { return Manifest != null; } }

        public ABBuildResult(AssetBundleManifest manifest)
        {
            Manifest =  manifest;
        }

		public override string ToString()
		{
			return "Manifest : == " + ( Manifest == null ? "null" : Manifest.name );
		}
	}
}

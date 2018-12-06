using UnityEngine;
using UnityEditor;
using Chipstar.Builder;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Chipstar.Downloads;
using System.Text;

namespace Chipstar.Builder
{
    public class SaveBuildMapPostProcess : ABBuildPostProcess<ABBuildData, ABBuildResult>
    {
        //=========================================
        //  プロパティ
        //=========================================
        private string FileName { get; set; }
        //=========================================
        //  関数
        //=========================================

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SaveBuildMapPostProcess(string name)
        {
            FileName = name;
        }

        /// <summary>
        /// ビルドマップを作成
        /// </summary>
        protected override void DoProcess( IABBuildConfig settings, ABBuildResult result, IList<ABBuildData> bundleList )
        {
            var json        = new BuildMapDataTable();
            var manifest    = result.Manifest;
			var prefix      = settings.RootFolder;
			json.Prefix		= prefix;

			using( var scope = new ProgressDialogScope( "Create Bundle Manifest : " + FileName, bundleList.Count ) )
			{
				for( int i = 0; i < bundleList.Count; i++ )
				{
					var data     = bundleList[ i ];
					var absPath  = Path.Combine( settings.OutputPath, data.ABName );
					var fileInfo = new FileInfo( absPath );
					var crc      = 0u;
					BuildPipeline.GetCRCForAssetBundle( absPath, out crc );
					var d = new BundleBuildData
					{
						ABName      = data.ABName,
						Assets      = data.Address,
						Hash        = manifest.GetAssetBundleHash( data.ABName ).ToString(),
						Crc			= crc,
						Dependencies= manifest.GetAllDependencies( data.ABName ),
						FileSize    = fileInfo.Length
					};

					scope.Show( data.ABName, i);
					json.Add( d );
				}
			}
			var addresses = bundleList
								.SelectMany(c => c.Address )
								.Distinct()
								.ToArray();

			using( var scope = new ProgressDialogScope( "Create Asset Table : " + FileName, addresses.Length) )
			{
				for( int i = 0; i < addresses.Length; i++)
				{
					var address = addresses[ i ];
					var path	= address.StartsWith( prefix ) ? address : prefix + address;
					var d = new AssetBuildData
					{
						Path = address,
						Guid = AssetDatabase.AssetPathToGUID( path )
					};
					scope.Show( address, i);
					json.Add( d );
				}
			}
            var saveFilePath= Path.Combine( settings.OutputPath, FileName );
            var contents    = JsonUtility.ToJson( json, true );
            //  書き込み
            File.WriteAllText( saveFilePath, contents );
        }
    }
}
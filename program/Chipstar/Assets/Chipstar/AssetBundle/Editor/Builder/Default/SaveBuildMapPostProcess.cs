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
	/// <summary>
	/// ビルドマップファイルを書き出す処理
	/// </summary>
    public class SaveBuildMapPostProcess : ABBuildPostProcess<ABBuildData, ABBuildResult>
    {
        //=========================================
        //  プロパティ
        //=========================================
        private string ManifestFileName { get; set; }
		private IManifestWriter Writer { get; }
		//=========================================
		//  関数
		//=========================================

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public SaveBuildMapPostProcess( string manifestName, IManifestWriter writer )
		{
            ManifestFileName = manifestName;
			Writer = writer;
        }
		public SaveBuildMapPostProcess(string manifestName) : this(manifestName, new RawTextJsonWriter(BuildMapDataTable.Encode)) { }

		/// <summary>
		/// ビルドマップを作成
		/// </summary>
		protected override void DoProcess( IABBuildConfig settings, ABBuildResult result, IList<ABBuildData> bundleList )
        {
            var json        = new BuildMapDataTable();
			var saveFilePath   = Path.Combine( settings.OutputPath, ManifestFileName );
			
			//	旧テーブルを取得
			//	アセットバージョンファイルを取得
			//	外部情報
			var manifest     = result.Manifest;
			var prefix       = settings.RootFolder;
			json.Prefix		 = prefix;

			using( var scope = new ProgressDialogScope( "Create Bundle Manifest : " + ManifestFileName, bundleList.Count ) )
			{
				//	テーブル作成
				for( int i = 0; i < bundleList.Count; i++ )
				{
					//	BuildFileData
					var fileData= bundleList[ i ];
					//	Path
					var absPath = Path.Combine( settings.OutputPath, fileData.ABName );
					//	Create BuildMap Data
					var d       = CreateBuildData( absPath, fileData, manifest );

					scope.Show( fileData.ABName, i);
					json.Add( d );
				}
			}
			var addresses = bundleList
								.SelectMany(c => c.Address )
								.Distinct()
								.ToArray();

			using( var scope = new ProgressDialogScope( "Create Asset Table : " + ManifestFileName, addresses.Length) )
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
			Writer.Write(saveFilePath, json);
		}

		/// <summary>
		/// 単一データ作成
		/// </summary>
		private BundleBuildData CreateBuildData( string absPath, ABBuildData buildFileData, AssetBundleManifest manifest )
		{
			var abName		 = buildFileData?.ABName;
			var crc          = FsUtillity.TryGetCrc( absPath );
			var hash         = manifest.TryGetHashString( abName );
			var dependencies = manifest.TryGetDependencies( abName );
			var size         = FsUtillity.TryGetFileSize( absPath );

			var d = new BundleBuildData
			{
				ABName      = abName,
				Assets      = buildFileData?.Address,
				Hash        = hash,
				Crc         = crc,
				Dependencies= dependencies,
				FileSize    = size,
				Labels      = buildFileData?.Labels,
			};

			return d;
		}
	}
}
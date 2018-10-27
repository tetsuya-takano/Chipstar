using UnityEngine;
using UnityEditor;
using Chipstar.Builder;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Chipstar.Example
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
            var json        = new BuildMapContents();
            var manifest    = result.Manifest;
            foreach (var data in bundleList)
            {
                var d = new BundleBuildData
                {
                    ABName      = data.ABName,
                    Assets      = data.Assets,
                    Hash        = manifest.GetAssetBundleHash( data.ABName ).ToString(),
                    Dependencies= manifest.GetAllDependencies( data.ABName )
                };
                json.AddBundle( d );
            }

            var assetPaths = bundleList.SelectMany(c => c.Assets).Distinct();
            foreach (var assetPath in assetPaths)
            {
                var d = new AssetBuildData
                {
                    Path = assetPath,
                    Guid = AssetDatabase.AssetPathToGUID( assetPath )
                };
                json.AddAsset( d );
            }
            var path        = Path.Combine( settings.OutputPath, FileName );
            var contents    = JsonUtility.ToJson( json, true );

            //  書き込み
            File.WriteAllText( path, contents, System.Text.Encoding.UTF8 );
        }
    }
}
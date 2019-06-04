using Chipstar.Builder;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Chipstar.Builder
{
    /// <summary>
    /// アセットバンドルマスタ用クラス
    /// </summary>
    public class ABPackageMst : ABPackageData<ABBuildData>
    {
		public override string Name { get { return PathPattern; } }

		public string	PathPattern { get; private set; }	// ファイルパス
		public string	PackName    { get; private set; }	// アセットバンドル名
        public string	MatchKey    { get; private set; }	// 名前の共通部分抜き出し用
		public string[]	Labels		{ get; private set; }	// グループラベル

        protected override void DoRead( string[] args )
        {
            PathPattern     = args[ 0 ];
            PackName        = args[ 1 ];
            Priority        = int.Parse( args[ 2 ] );
			Labels			= string.IsNullOrEmpty( args[ 3 ] ) 
								? new string[ 0 ] 
								: args[ 3 ].Split( '/'  );

            PathFilter      = new SingleWildCardPathFilter  ( PathPattern );
            NameConverter   = new NameOverrideConverter     ( PackName,".ab" );
        }
		protected override ABBuildData DoBuild( string rootFolder, string name, string[] assets )
		{
			var data = base.DoBuild( rootFolder, name, assets );
			data.SetLabel( Labels );
			return data;
		}
	}

    /// <summary>
    ///  アセットバンドル対象フォルダ設定エクセルの管理クラス
    /// </summary>
    public class ABPackageMstTable : ABPackageSettings<ABPackageMst, ABBuildData>
    {
		public ABPackageMstTable( string path )
        {
            Path = path;
        }
	}
}
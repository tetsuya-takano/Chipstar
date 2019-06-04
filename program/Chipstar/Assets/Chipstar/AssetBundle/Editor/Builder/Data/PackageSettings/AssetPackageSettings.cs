using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;

namespace Chipstar.Builder
{
    /// <summary>
    /// アセットバンドルのパッケージ化設定のインターフェース
    /// </summary>
    public interface IABPackageSettings<TPackageData, TBuildData> 
        where TPackageData : 
              IABPackageData<TBuildData> where TBuildData : IABBuildData
    {
        IList<TPackageData> CreatePackageList();
    }

    /// <summary>
    /// アセットのアセットバンドル固め方設定データ
    /// </summary>
    public interface IABPackageData<T> where T : IABBuildData
    {
        string      Name        { get; }
        int         Priority    { get; }    //	優先順位
        bool        IsMatch( string rootFolder, string filePath);
        IList<T>    Build  ( string rootFolder, IEnumerable<string> packagedAssets );
        void        Read   ( string line );
    }

    public interface IABBuildData
    {
        string      ABName	{ get; }
        string[]    Assets	{ get; }
		string[]    Address { get; }
		void Apply( string rootFolder, string name, string[] assets );
		AssetBundleBuild ToBuildEntry();
		void Merge<TBuildData>( TBuildData b ) where TBuildData : IABBuildData;
	}


	/// <summary>
	/// アセットバンドル固めデータ
	/// バンドル名 ＆ アセット一覧が基本
	/// </summary>
	public class ABBuildData : IABBuildData
	{
		private string	FolderPrefix { get; set; }
		public string	ABName	{ get; protected set; }
		public string[] Assets	{ get; protected set; }
		public string[] Address { get; protected set; }
		public string[] Labels	{ get; protected set; }

		/// <summary>
		/// データ保持と必要なら変換
		/// </summary>
		public virtual void Apply( string rootFolder, string name, string[] assets )
		{
			FolderPrefix=   string.Intern( rootFolder );
			ABName      =   name;
			Assets      =   assets;
			Address     =   ToAddress( assets );
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual AssetBundleBuild ToBuildEntry()
		{
			return new AssetBundleBuild
			{
				assetBundleName = ABName,
				assetNames      = Assets,
				addressableNames= Address,
			};
		}
		private string[] ToAddress( string[] assetPaths )
		{
			var adresses = new string[assetPaths.Length];
			for( int i = 0; i < Assets.Length; i++ )
			{
				var path        = Assets[ i ];
				//	MEMO : シーンアセットはロード用アドレスを変更できないらしい
				//		   通常のアセットはビルド先ルートフォルダを削る
				var isSceneAsset=  AssetDatabase.GetMainAssetTypeAtPath( path ) == typeof(SceneAsset);
				adresses[i]   = isSceneAsset ? path : path.Replace( FolderPrefix, string.Empty );
			}
			return adresses;
		}
		/// <summary>
		/// マージ
		/// </summary>
		public void Merge<TBuildData>( TBuildData b ) where TBuildData : IABBuildData
		{
			Address = Address.Union( b.Address ).ToArray();
			Assets  = Assets.Union( b.Assets ).ToArray();
			DoMarge( b );
		}
		protected virtual void DoMarge<TBuildData>( TBuildData b ) where TBuildData : IABBuildData
		{
		}
		/// <summary>
		/// ラベルのセット
		/// </summary>
		public void SetLabel( string[] labels )
		{
			Labels = labels;
		}
	}

    public abstract class ABPackageData<T> : IABPackageData<T> 
        where T : IABBuildData, new()
    {
        //==========================
        //  プロパティ
        //==========================
        public abstract string   Name     { get; }
        public virtual  int      Priority { get; protected set; }

        protected IABNameConverter NameConverter   { get; set; }
        protected IABPathFilter    PathFilter      { get; set; }

        //==========================
        //  関数
        //==========================

        public virtual bool IsMatch( string rootFolder, string path )
        {
            return DoMatchTargetPath( rootFolder, path );
        }

        protected virtual bool DoMatchTargetPath( string rootFolder, string path )
        {
            return PathFilter.IsMatch( rootFolder, path );
        }

        public IList<T> Build( string rootFolder, IEnumerable<string> packagedAssets )
        {
			var groups  = packagedAssets
							.GroupBy
							( c => NameConverter
										.Convert( c )
										.Replace( rootFolder, string.Empty ) 
										.ToLower()
							).ToArray();

			var list    = new List<T>();
			using( var scope = new ProgressDialogScope( "Build Package", groups.Length ) )
			{
				for( int i = 0; i < groups.Length; i++ )
				{
					var item    = groups[i];
					var name    = item.Key;
					var assets  = item.ToArray();
					var data    = DoBuild( rootFolder, name, assets );
					list.Add( data );

					scope.Show( name, i );
				}
			}
            return list;
        }

        protected virtual T DoBuild( string rootFolder, string name, string[] assets )
        {
            var data = new T();

			data.Apply( rootFolder, name, assets );

			return data;
        }

        public virtual void Read( string line )
        {
            DoRead( line.Split( ',' ) );
        }
        protected abstract void DoRead( string[] args );
    }

    public class ABPackageSettings<TPack, TBuild> 
        : IABPackageSettings<TPack, TBuild> 
            where TBuild: IABBuildData
            where TPack : IABPackageData<TBuild>, new()
    {
        protected string Path { get; set; }

        public virtual IList<TPack> CreatePackageList()
        {
            var list = new List<TPack>();
			var lines = File.ReadAllLines( Path );
            
			foreach( var line in lines )
			{
				if( string.IsNullOrEmpty( line ))
				{
					continue;
				}
				if (IsComment(line))
				{
					continue;
				}
				if ( IsSkipPattern( line ) )
                {
                    continue;
                }
                var pack = DoBuild( line );
                list.Add( pack );
            }

            return list;
        }

        protected virtual bool IsSkipPattern( string line ) { return line == ",,0,,"; }
		protected virtual bool IsComment(string line) { return line.ElementAtOrDefault( 0 ) == '#'; }

		protected virtual TPack DoBuild( string line)
        {
            var d = new TPack();

            d.Read( line );

            return d;
        }
    }
}
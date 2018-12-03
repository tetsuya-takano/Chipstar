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
        bool        IsMatch( string path );
        IList<T>    Build  ( IEnumerable<string> packagedAssets );
        void        Read   ( string line );
    }

    public interface IABBuildData
    {
        string      ABName { get; set; }
        string[]    Assets { get; set; }

		AssetBundleBuild ToBuildEntry();
	}


    public class ABBuildData : IABBuildData
    {
        public string   ABName { get; set; }
        public string[] Assets { get; set; }

		public AssetBundleBuild ToBuildEntry()
		{
			return new AssetBundleBuild
			{
				assetBundleName = ABName,
				assetNames      = Assets,
			};
		}
    }

    public abstract class ABPackageData<T> : IABPackageData<T> 
        where T : IABBuildData, new()
    {
        //==========================
        //  プロパティ
        //==========================
        public virtual string   Name     { get; protected set; }
        public virtual int      Priority { get; protected set; }

        protected IABNameConverter NameConverter   { get; set; }
        protected IABPathFilter    PathFilter      { get; set; }

        //==========================
        //  関数
        //==========================

        public virtual bool IsMatch( string path )
        {
            return DoMatchTargetPath( path );
        }

        protected virtual bool DoMatchTargetPath( string path )
        {
            return PathFilter.IsMatch( path );
        }

        public IList<T> Build( IEnumerable<string> packagedAssets )
        {
            var groups  = packagedAssets.GroupBy( c => NameConverter.Convert( c ));
            var list    = new List<T>();
            foreach( var item in groups )
            {
                var name    = item.Key;
                var assets  = item.ToArray();
                var data    = DoBuild( name, assets );
                list.Add( data );
            }
            return list;
        }

        protected virtual T DoBuild( string name, string[] assets )
        {
            var data = new T();

            data.ABName = name;
            data.Assets = assets;

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
            using( var stream = new StreamReader( Path ) )
            {
                for( 
                    var line = stream.ReadLine();
                    !string.IsNullOrEmpty( line ); 
                    line     = stream.ReadLine() )
                {
                    if( IsSkipPattern( line ) )
                    {
                        continue;
                    }
                    var pack = DoBuild( line );
                    list.Add( pack );
                }
            }

            return list;
        }

        protected virtual bool IsSkipPattern( string line ) { return line == ",,0,,"; }

        protected virtual TPack DoBuild( string line)
        {
            var d = new TPack();

            d.Read( line );

            return d;
        }
    }
}
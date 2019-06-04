using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Chipstar.Builder
{
	/// <summary>
	/// 不要になったアセットバンドルを削除する
	/// </summary>
	public class DeleteUnusedBundlePostProcess : ABBuildPostProcess<ABBuildData, ABBuildResult>
	{
		/// <summary>
		/// 実行
		/// </summary>
		protected override void DoProcess( IABBuildConfig settings, ABBuildResult result, IList<ABBuildData> bundleList )
		{
			//	親ディレクトリ
			var rootDirPath = settings.OutputPath;
			if( !Directory.Exists( rootDirPath ) )
			{
				Debug.LogWarning( $"OutputDir is Not Exists : { rootDirPath}" );
				return;
			}

			//	ファイル一覧
			var fileList    = Directory.EnumerateFiles( 
								path			: rootDirPath, 
								searchPattern	:"*.ab", 
								searchOption	: SearchOption.AllDirectories 
							)
							.Select( c => c.Replace( "\\","/" ) )
							.OrderBy( c => c )
							.ToList();
			// 出力結果にあるものは除外していく
			foreach( var ab in bundleList )
			{
				var abPath = Path.Combine( rootDirPath, ab.ABName ).Replace("\\","/");
				if( File.Exists( abPath ))
				{
					fileList.Remove( abPath );
				}
			}

			var builder = new StringBuilder();
			//	不要になったファイルを削除
			foreach( var f in fileList )
			{
				builder.AppendLine( $"delete :: {f}" );
				SafeDelete( f );
				SafeDelete( f + ".manifest" );
			}
			File.WriteAllText( Path.Combine( rootDirPath, "delete-log.txt" ), builder.ToString(), new UTF8Encoding( false ) );
		}

		private void SafeDelete( string path )
		{
			if( !File.Exists( path ) )
			{
				return;
			}
			File.Delete( path );
		}
	}
}
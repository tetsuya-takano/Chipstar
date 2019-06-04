using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
namespace Chipstar.Builder
{
	/// <summary>
	/// アセットバージョンを更新する事前処理
	/// </summary>
	public class AssetVersionBuildPreProcess<T> : ABBuildPreProcess<T> where T : IABBuildData
	{
		//=====================================
		//	変数
		//=====================================
		private readonly string m_fileName = string.Empty;

		//=====================================
		//	関数
		//=====================================

		public AssetVersionBuildPreProcess( string fileName )
		{
			m_fileName = fileName;
		}

		/// <summary>
		/// 処理
		/// </summary>
		protected override void DoProcess( IABBuildConfig config, IList<T> bundleList )
		{
			//	出力先
			var path  = Path.Combine( config.OutputPath, m_fileName );
			//	取得
			var table =  AssetVersionTable.Read( path );

			//	更新
			table.Push();

			//	書き込み
			AssetVersionTable.Write( path, table );
		}
	}
}
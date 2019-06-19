using Chipstar.Downloads;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

namespace Chipstar.Downloads.CriWare
{
	/// <summary>
	/// CRIのインストール処理
	/// </summary>
	public class InstallJob : LoadJob<CriInstallHandler, CriFsInstallRequest, Empty>
	{
		//====================================
		//	プロパティ
		//====================================
		private IAccessLocation DestLocation { get; set; }

		//====================================
		//	関数
		//====================================

		public InstallJob( IAccessLocation srcLocation, IAccessLocation destLocation ) : base( srcLocation, new CriInstallHandler() )
		{
			DestLocation = destLocation;
		}

		/// <summary>
		/// 実行時
		/// </summary>
		protected override void DoRun( IAccessLocation location )
		{
			var folder = Path.GetDirectoryName( DestLocation.FullPath );
			//CriUtils.WarningDetail( folder );
			if( !Directory.Exists( folder ))
			{
				Directory.CreateDirectory( folder );
			}
			//CriUtils.WarningDetail( "source : {0}\ndest{1}", location.FullPath, DestLocation.FullPath );
			Source = CriFsUtility.Install( location.FullPath, DestLocation.FullPath );
		}


		/// <summary>
		/// 進捗
		/// </summary>
		protected override float GetProgress( CriFsInstallRequest source )
		{
			return source.progress;
		}
		/// <summary>
		/// 完了判定
		/// </summary>
		protected override bool GetIsComplete( CriFsInstallRequest source )
		{
			return source.isDone;
		}
		/// <summary>
		/// エラー判定
		/// </summary>
		protected override bool GetIsError( CriFsInstallRequest source )
		{
			return source.error != null && source.isDone;
		}
	}

	/// <summary>
	/// Criインストールのハンドリング
	/// </summary>
	public class CriInstallHandler : DLHandler<CriFsInstallRequest, Empty>
	{
		/// <summary>
		/// 完了時
		/// </summary>
		protected override Empty DoComplete( CriFsInstallRequest source )
		{
			return Empty.Default;
		}
	}
}
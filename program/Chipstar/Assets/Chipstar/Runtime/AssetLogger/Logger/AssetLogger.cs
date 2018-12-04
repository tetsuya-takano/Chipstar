using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chipstar.Downloads
{
	/// <summary>
	/// ログ機能
	/// </summary>
	public interface IAssetLogger : IDisposable
	{
		void Log	( string log );
		void Error	( string log );
		void Warning( string log );
		bool CanLog ( int logLevel );
	}
	/// <summary>
	/// アセット読み込みログ
	/// </summary>
	public interface ILoadAssetLog : IAssetLogger { }

	public interface ILoadLevelLog		: IAssetLogger { }
	public interface ILoadAdditiveLog	: IAssetLogger { }
	/// <summary>
	/// シーン読み込みログ
	/// </summary>
	public interface ILoadSceneLog		: ILoadAdditiveLog, ILoadLevelLog { }

	/// <summary>
	/// ログベースクラス
	/// </summary>
	public abstract class AssetLogger : IAssetLogger
	{
		//================================
		//	プロパティ
		//================================
		protected int ViewLevel { get; set; }

		//================================
		//	関数
		//================================

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public AssetLogger( int level )
		{
			ViewLevel = level;
		}

		/// <summary>
		/// 破棄
		/// </summary>
		public void Dispose()
		{

		}
		protected virtual void DoDispose() { }

		/// <summary>
		/// ログを出すかどうか
		/// </summary>
		public bool CanLog( int logLevel )
		{
			return DoCanlog( logLevel );
		}
		/// <summary>
		/// エラー
		/// </summary>
		public void Error( string log )
		{
			DoError( log );
		}
		/// <summary>
		/// ログ
		/// </summary>
		public void Log( string log )
		{
			DoLog( log );
		}

		/// <summary>
		/// 警告
		/// </summary>
		public void Warning( string log )
		{
			DoWarning( log );
		}


		protected virtual bool DoCanlog( int level )
		{
			return ViewLevel <= level;
		}
		protected virtual void DoLog	( string log ) { }
		protected virtual void DoWarning( string log ) { }
		protected virtual void DoError	( string log ) { }
	}
}
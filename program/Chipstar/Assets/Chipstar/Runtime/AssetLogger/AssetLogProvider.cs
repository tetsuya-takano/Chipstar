using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chipstar.Downloads
{
	public interface IAssetLogProvider : IDisposable
	{
		void Log	<TLogger>( string log ) where TLogger : IAssetLogger;
		void Warning<TLogger>( string log ) where TLogger : IAssetLogger;
		void Error	<TLogger>( string log ) where TLogger : IAssetLogger;
	}
	/// <summary>
	/// ログを出さない
	/// </summary>
	public sealed class NothingLogger : IAssetLogProvider
	{
		public void Dispose() { }
		public void Error<TLogger>	( string log ) where TLogger : IAssetLogger { }
		public void Log<TLogger>	( string log ) where TLogger : IAssetLogger { }
		public void Warning<TLogger>( string log ) where TLogger : IAssetLogger { }
	}
	/// <summary>
	/// ロガー
	/// </summary>
	public class AssetLogProvider
		: IAssetLogProvider
	{
		//===================================
		//	class
		//===================================

		//===================================
		//	変数
		//===================================
		private IAssetLogger[] m_loggers = null;

		//===================================
		//	プロパティ
		//===================================
		public int LogLevel { get; set; }

		//===================================
		//	関数
		//===================================

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public AssetLogProvider( params IAssetLogger[] args )
		{
			m_loggers = args;
		}

		/// <summary>
		/// 破棄処理
		/// </summary>
		public void Dispose()
		{

		}
		/// <summary>
		/// ログ表示
		/// </summary>
		public void Log<TLogger>( string log ) where TLogger : IAssetLogger
		{
			foreach( var logger in m_loggers )
			{
				//	あるかどうか
				if( !( logger is TLogger	 )) { continue; }
				if( !logger.CanLog( LogLevel )) { continue; }
				logger.Log( log );
			}
		}
		/// <summary>
		/// 警告
		/// </summary>
		public void Warning<TLogger>( string log ) where TLogger : IAssetLogger
		{
			foreach( var logger in m_loggers )
			{
				//	あるかどうか
				if( !( logger is TLogger ) ) { continue; }
				if( !logger.CanLog( LogLevel ) ) { continue; }
				logger.Warning( log );
			}
		}
		/// <summary>
		/// エラー
		/// </summary>
		public void Error<TLogger>( string log ) where TLogger : IAssetLogger
		{
			foreach( var logger in m_loggers )
			{
				//	あるかどうか
				if( !( logger is TLogger ) ) { continue; }
				if( !logger.CanLog( LogLevel ) ) { continue; }
				logger.Error( log );
			}
		}
	}
}
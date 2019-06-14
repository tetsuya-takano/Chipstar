#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Chipstar.Downloads
{
	/// <summary>
	/// エディタ用のローダー
	/// 実際に取得できないタイミングの場合は警告を出す
	/// </summary>
	public sealed class EditorFactoryContainer : IFactoryContainer
	{
		//=====================================
		//	変数
		//=====================================
		private IAssetLoadFactory	m_afterLoginAssetFact;
		private IAssetLoadFactory	m_alwaysAssetFact;
		private ISceneLoadFactory	m_afterLoginSceneFact;
		private ISceneLoadFactory	m_alwaysSceneFact;
		private bool				m_isLogin;

		//=====================================
		//	関数
		//=====================================
		public EditorFactoryContainer(
			IAssetLoadFactory afterLoginAssetFact,
			IAssetLoadFactory alwaysAssetFact,
			ISceneLoadFactory afterLoginSceneFact,
			ISceneLoadFactory alwaysSceneFact )
		{
			m_afterLoginAssetFact= afterLoginAssetFact;
			m_alwaysAssetFact	 = alwaysAssetFact;
			m_afterLoginSceneFact= afterLoginSceneFact;
			m_alwaysSceneFact	 = alwaysSceneFact;
		}

		public void Dispose()
		{
			m_afterLoginAssetFact	.Dispose();
			m_alwaysAssetFact		.Dispose();
			m_afterLoginSceneFact	.Dispose();
			m_alwaysSceneFact		.Dispose();

			m_afterLoginAssetFact	= null;
			m_alwaysAssetFact		= null;
			m_afterLoginSceneFact	= null;
			m_alwaysSceneFact		= null;
		}
		private T Get<T>( string path, T afterLogin, T always ) where T : ILoadOperateFactory
		{
			if( afterLogin.CanLoad( path ) )
			{
				if( !m_isLogin )
				{
					//	ログイン前なので警告を出力
					ChipstarLog.Log_WarningAccessAfterLoginAsset( path );
				}
				return afterLogin;
			}
			return always;
		}

		/// <summary>
		/// アセット用のLoaderを取得
		/// </summary>
		public IAssetLoadFactory GetFromAsset( string path )
		{
			return Get( 
				path		: path,
				afterLogin	: m_afterLoginAssetFact, 
				always		: m_alwaysAssetFact );
		}

		/// <summary>
		/// Scene用のファクトリを取得
		/// </summary>
		public ISceneLoadFactory GetFromScene( string path )
		{
			return Get( 
				path		: path,
				afterLogin	: m_afterLoginSceneFact, 
				always		: m_alwaysSceneFact );
		}

		/// <summary>
		/// 
		/// </summary>
		public void SetLoginMode( bool isLogin )
		{
			m_isLogin = isLogin;
		}
	}
}
#endif
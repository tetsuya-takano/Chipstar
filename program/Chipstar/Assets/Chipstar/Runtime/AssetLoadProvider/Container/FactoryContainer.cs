using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chipstar.AssetLoad
{
	public interface IFactoryContainer
	{
		T		Get<T>( string path ) where T : ILoadOperateFactory;
	}
	/// <summary>
	/// リクエスト作成クラスのコンテナ
	/// </summary>
	public class FactoryContainer : IFactoryContainer
	{
		//===============================
		//	変数
		//===============================
		private List<ILoadOperateFactory> m_factories = new List<ILoadOperateFactory>();

		//===============================
		//	関数
		//===============================

		/// <summary>
		/// 使用可能なものを取得
		/// </summary>
		public T Get<T>( string path ) where T : ILoadOperateFactory
		{
			for( int i = 0; i < m_factories.Count; i++ )
			{
				//	型チェック
				var factory = m_factories[ i ];
				if( !( factory is T ) )
				{
					continue;
				}
				//	取得可能なら通す
				if( factory.CanLoad( path ) )
				{
					return (T)factory;
				}
			}
			return default(T);
		}
	}
}
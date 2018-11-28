using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chipstar.AssetLoad
{
	public interface IFactoryContainer : IDisposable
	{
		T Get<T>( string path ) where T : ILoadOperateFactory;
	}
	/// <summary>
	/// リクエスト作成クラスのコンテナ
	/// </summary>
	public class FactoryContainer : IFactoryContainer
	{
		//===============================
		//	変数
		//===============================
		private List<ILoadOperateFactory> m_factories = null;

		//===============================
		//	関数
		//===============================

		public FactoryContainer( params ILoadOperateFactory[] factories )
		{
			m_factories = new List<ILoadOperateFactory>( factories );
		}

		/// <summary>
		/// 破棄処理
		/// </summary>
		public void Dispose()
		{
			foreach( var f in m_factories)
			{
				f.Dispose();
			}
			m_factories.Clear();
		}

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
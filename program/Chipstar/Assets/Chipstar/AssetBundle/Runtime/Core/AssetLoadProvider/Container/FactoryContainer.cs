using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chipstar.Downloads
{
	public interface IFactoryContainer : IDisposable
	{
		ISceneLoadFactory GetFromScene( string path );
		IAssetLoadFactory GetFromAsset( string path );
	}
	/// <summary>
	/// リクエスト作成クラスのコンテナ
	/// </summary>
	public class FactoryContainer : IFactoryContainer
	{
		//===============================
		//	変数
		//===============================
		private IAssetLoadFactory[] m_assets = null;
		private ISceneLoadFactory[] m_scenes = null;

		private List<ILoadOperateFactory> m_list = new List<ILoadOperateFactory>();

		//===============================
		//	関数
		//===============================

		public FactoryContainer( 
			IAssetLoadFactory[] assets,
			ISceneLoadFactory[] scenes
		)
		{
			m_assets = assets;
			m_scenes = scenes;
		}

		/// <summary>
		/// 破棄処理
		/// </summary>
		public void Dispose()
		{
			foreach( var f in m_assets)
			{
				f.Dispose();
			}
			foreach( var f in m_scenes )
			{
				f.Dispose();
			}

			m_assets = null;
			m_scenes = null;
			m_list.Clear();
		}

		/// <summary>
		/// 使用可能なものを取得
		/// </summary>
		private T Get<T>( string path, IList<T> list ) where T : ILoadOperateFactory
		{
			m_list.Clear();
			for( int i = 0; i < list.Count; i++ )
			{
				//	型チェック
				var factory = list[ i ];
				if( !( factory is T ) )
				{
					continue;
				}
				//	取得可能なら通す
				if( factory.CanLoad( path ) )
				{
					m_list.Add( factory );
				}
			}
			if( m_list.Count > 0 )
			{
				T fact = (T)m_list[ 0 ];
				for( int i = 0; i < m_list.Count;i++)
				{
					var f = (T)m_list[ i ];
					if( f.Priority > fact.Priority)
					{
						fact = f;
					}
				}
				return fact;
			}
			throw new Exception( string.Format( "{0}({1})\nをロード出来る機能がありません", path, typeof( T ) ) );
		}

		public IAssetLoadFactory GetFromAsset( string path )
		{
			return Get<IAssetLoadFactory>( path, m_assets );
		}

		public ISceneLoadFactory GetFromScene( string path )
		{
			return Get<ISceneLoadFactory>( path, m_scenes );
		}
	}
}
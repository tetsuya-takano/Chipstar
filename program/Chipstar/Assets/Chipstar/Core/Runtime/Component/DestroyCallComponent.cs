using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chipstar
{
	/// <summary>
	/// Destroy時に登録したDisposeを呼ぶコンポーネント
	/// </summary>
	[DisallowMultipleComponent]
	public sealed class DestroyCallComponent : MonoBehaviour
	{
		//=====================================
		//	変数
		//=====================================
		private List<IDisposable> m_disposeList = new List<IDisposable>();

		//=====================================
		//	関数
		//=====================================
		private void OnDestroy()
		{
			if( m_disposeList.Count <= 0)
			{
				return;
			}
			for( int i = 0; i < m_disposeList.Count;i++)
			{
				m_disposeList[i].DisposeIfNotNull();
			}
			m_disposeList.Clear();
		}

		public void AddDisposable( IDisposable disposable )
		{
			if( !this )
			{
				return;
			}
			m_disposeList.Add(disposable);
		}
	}

	public static class DestroyCallComponentExtensions
	{
		private static DestroyCallComponent GetOrAdd( GameObject go )
		{
			if( !go)
			{
				throw new NullReferenceException("Component is Null");
			}
			var c = go.GetComponent<DestroyCallComponent>();
			if( c )
			{
				return c;
			}
			return go.AddComponent<DestroyCallComponent>();
		}
		private static DestroyCallComponent GetOrAdd( Component obj)
		{
			return GetOrAdd( obj.gameObject );
		}


		public static IDisposable AddTo( this IDisposable self ,GameObject obj )
		{
			if( self == null)
			{
				return self;
			}
			if( !obj )
			{
				self.DisposeIfNotNull();
				return self;
			}
			var c = GetOrAdd(obj);
			c.AddDisposable(self);
			return self;
		}
		public static IDisposable AddTo(this IDisposable self, Component obj)
		{
			if( self == null )
			{
				return self;
			}
			if (!obj)
			{
				self.DisposeIfNotNull();
				return self;
			}
			var c = GetOrAdd(obj);
			c.AddDisposable(self);
			return self;
		}
	}
}
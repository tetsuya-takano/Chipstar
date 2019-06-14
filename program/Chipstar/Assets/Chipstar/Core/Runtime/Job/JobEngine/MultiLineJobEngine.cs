using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace Chipstar.Downloads
{
	/// <summary>
	/// 並列ジョブエンジン
	/// </summary>
	public class MultiLineJobEngine : IJobEngine
	{
		//=====================================
		//	変数
		//=====================================
		private int m_index = 0;
		private JobEngine[] m_engineList = new JobEngine[ 0 ];

		//=====================================
		//	関数
		//=====================================

		public MultiLineJobEngine( int lineCount )
		{
			m_engineList = new JobEngine[ lineCount ];
			for (int i = 0; i < m_engineList.Length; i++)
			{
				m_engineList[i] = new JobEngine();
			}
		}

		public void Cancel()
		{
			foreach( var j in m_engineList)
			{
				j.Cancel();
			}
		}

		public void Dispose()
		{
			foreach (var j in m_engineList)
			{
				j.DisposeIfNotNull();
			}
		}

		public void Enqueue(ILoadJob request)
		{
			var engine = m_engineList[ m_index ];
			engine.Enqueue( request );
			m_index = (m_index + 1) % m_engineList.Length;
		}

		public bool HasRequest(IAccessLocation location)
		{
			foreach (var j in m_engineList)
			{
				if( j.HasRequest( location ))
				{
					return true;
				}
			}
			return false;
		}

		public void Update()
		{
			foreach( var j in m_engineList)
			{
				j.Update();
			}
		}
	}
}
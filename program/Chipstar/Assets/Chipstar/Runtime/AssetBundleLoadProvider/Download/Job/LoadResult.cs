using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Chipstar.Downloads
{
	public sealed class Empty
	{
		public static readonly Empty Default = new Empty();
	}
    public interface ILoadResult : IDisposable
    {
        bool    IsCompleted { get; }
    }

	/// <summary>
	/// 結果を取るロード処理
	/// </summary>
	public sealed class LoadResult<T> : ILoadResult
	{
        //=====================================
        //  変数
        //=====================================
        private ILoadJob<T>         m_job           = null;

        //=====================================
        //  プロパティ
        //=====================================
        public			T       Content     { get; private set; }
        public			bool    IsCompleted { get; private set; }

		//=====================================
		//  関数
		//=====================================

		public LoadResult(
            ILoadJob<T>		job, 
            Action<T>		onCompleted
        )
        {
            m_job           = job;
            m_job.OnLoaded  = () =>
            {
                IsCompleted = true;
                Content     = m_job.Content;
                onCompleted( Content );
            };
        }
        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            m_job           = null;
        }
    }

	/// <summary>
	/// ロードをしないけど積む
	/// </summary>
	public sealed class LoadSkip : ILoadResult
	{
		public static readonly LoadSkip Default = new LoadSkip();
		public bool IsCompleted { get { return true; } }
		public void Dispose()	{ }
	}
	/// <summary>
	/// ロード結果処理を直列にする
	/// </summary>
	public sealed class JoinLoadResult : ILoadResult
    {
        //================================
        //  変数
        //================================
        private ILoadResult	m_prev = null;
        private ILoadResult m_next = null;

        //================================
        //  プロパティ
        //================================
        public bool     IsCompleted
		{
			get
			{
				if( !m_prev.IsCompleted )
				{
					return false;
				}
				if( !m_next.IsCompleted )
				{
					return false;
				}
				return true;
			}
		}

        //================================
        //  関数
        //================================

        public JoinLoadResult( ILoadResult prev, Func<ILoadResult> onNext )
        {
            m_prev = prev;
            m_next = onNext();
        }

        /// <summary>
        /// 破棄処理
        /// </summary>
        public void Dispose()
        {
            m_prev.Dispose();
            m_next.Dispose();
        }
    }
	/// <summary>
	/// 並列で待つ
	/// </summary>
    public sealed class ParallelLoadResult : ILoadResult
    {
     //================================
        //  変数
        //================================
        private ILoadResult[]	m_list  = null;
        //================================
        //  プロパティ
        //================================
        public bool     IsCompleted
		{
			get
			{
				for( var i = 0; i < m_list.Length; i++ )
				{
					if( !m_list[i].IsCompleted )
					{
						return false;
					}
				}
				return true;
			}
		}

        //================================
        //  関数
        //================================

        public ParallelLoadResult( ILoadResult[] args )
        {
            m_list      = args;
        }

        /// <summary>
        /// 破棄処理
        /// </summary>
        public void Dispose()
        {
            foreach( var r in m_list )
            {
                r.Dispose();
            }
            m_list      = null;
        }
    }

	/// <summary>
	/// コルーチン化する
	/// </summary>
	public sealed class LoadResultYieldInstruction : CustomYieldInstruction
	{
		//========================================
		//	変数
		//========================================
		private ILoadResult m_self = null;

		//========================================
		//	プロパティ
		//========================================
		public override bool keepWaiting { get { return m_self != null && !m_self.IsCompleted; } }

		//========================================
		//	関数
		//========================================
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public LoadResultYieldInstruction( ILoadResult result )
		{
			m_self = result;
		}
	}

	/// <summary>
	/// 合成関連の拡張
	/// </summary>
    public static class ILoadResultExtensions
    {
		/// <summary>
		/// 並列
		/// </summary>
        public static ILoadResult ToParallel(this ILoadResult[] self)
        {
            return new ParallelLoadResult( self );
        }
        
		/// <summary>
		/// 直列
		/// </summary>
        public static ILoadResult ToJoin( this ILoadResult self, Func<ILoadResult> onNext )
        {
            return new JoinLoadResult( self, onNext );
        }
		/// <summary>
		/// 
		/// </summary>
		public static LoadResultYieldInstruction ToYieldInstruction( this ILoadResult self )
		{
			return new LoadResultYieldInstruction( self );
		}
	}
}

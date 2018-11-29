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
        Action  OnCompleted { set; }
    }

	public sealed class LoadResult<T> : ILoadResult
	{
        //=====================================
        //  変数
        //=====================================
        private ILoadJob<T>         m_job           = null;

        //=====================================
        //  プロパティ
        //=====================================
        public			Action  OnCompleted { private get; set; }
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
				if( OnCompleted != null )
				{
					OnCompleted();
				}
            };
        }
        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            m_job           = null;
            OnCompleted     = null;
        }
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
        public bool     IsCompleted { get; private set; }
        public Action   OnCompleted { set; private get; }

        //================================
        //  関数
        //================================

        public JoinLoadResult( ILoadResult prev, Func<ILoadResult> onNext )
        {
            m_prev = prev;
            m_prev.OnCompleted = () =>
            {
                m_next = onNext();
                m_next.OnCompleted = () =>
                {
                    IsCompleted = true;
                    if (OnCompleted != null)
                    {
                        OnCompleted();
                    }
                };
            };
        }

        /// <summary>
        /// 破棄処理
        /// </summary>
        public void Dispose()
        {
            m_prev.Dispose();
            if( m_next != null )
            {
                m_next.Dispose();
            }
            OnCompleted = null;
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
        private ILoadResult[	] m_list  = null;
        private int              m_compCount   = 0;
        //================================
        //  プロパティ
        //================================
        public bool     IsCompleted { private set; get; }
        public Action   OnCompleted { set; private get; }

        //================================
        //  関数
        //================================

        public ParallelLoadResult( ILoadResult[] args )
        {
            m_list      = args;
            m_compCount = 0;
            Action<ParallelLoadResult> onDoneCallback = (self) =>
            {
                var i = self.m_compCount;
                self.m_compCount++;
                if( self.m_compCount < self.m_list.Length )
                {
                    return;
                }
                IsCompleted = true;
                self.OnCompleted();
            };

            foreach( var ret in m_list )
            {
                ret.OnCompleted = () => onDoneCallback( this );
            }
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
            OnCompleted = null;
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

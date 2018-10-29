using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Chipstar.Downloads
{
    public interface ILoadResult : IDisposable
    {
        Action OnCompleted { set; }
    }

    public interface ICompositeLoadResult : ILoadResult
    {
    }

    public sealed class LoadResult<T> : ILoadResult
    {
        //=====================================
        //  変数
        //=====================================
        private IDisposable         m_dispose       = null;
        private ILoadJob<T>         m_job           = null;

        //=====================================
        //  プロパティ
        //=====================================

        public Action OnCompleted { private get; set; }

        //=====================================
        //  関数
        //=====================================

        public LoadResult(
            ILoadJob<T>             job, 
            Action<ILoadTask<T>>    onCompleted, 
            IDisposable             dispose 
        )
        {
            m_job           = job;
            m_job.OnLoaded  = () =>
            {
                onCompleted( m_job );
                OnCompleted( );
            };
            m_dispose       = dispose;
        }
        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            m_job           = null;
            m_dispose.Dispose();
            m_dispose       = null;
            OnCompleted     = null;
        }
    }

    /// <summary>
    /// ロード結果処理を直列にする
    /// </summary>
    public sealed class JoinLoadResult : ICompositeLoadResult
    {
        //================================
        //  変数
        //================================
        private ILoadResult m_prev = null;
        private ILoadResult m_next = null;

        //================================
        //  プロパティ
        //================================
        public Action OnCompleted { set; private get; }

        //================================
        //  関数
        //================================

        public JoinLoadResult( ILoadResult prev, Func<ILoadResult> onNext )
        {
            m_prev = prev;
            m_prev.OnCompleted = () =>
            {
                m_next = onNext();
                m_next.OnCompleted = OnCompleted;
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

    public sealed class ParallelLoadResult : ICompositeLoadResult
    {
        //================================
        //  変数
        //================================
        private ILoadResult[] m_list        = null;
        private int           m_compCount   = 0;
        //================================
        //  プロパティ
        //================================
        public Action OnCompleted { set; private get; }

        //================================
        //  関数
        //================================

        public ParallelLoadResult( ILoadResult[] args )
        {
            m_list      = args;
            m_compCount = m_list.Length;
            Action<ParallelLoadResult> onCopletedOnce = (result) =>
            {
                result.m_compCount--;
                if( result.m_compCount > 0 )
                {
                    return;
                }
                result.OnCompleted();
            };

            foreach( var ret in m_list )
            {
                ret.OnCompleted = () => onCopletedOnce( this );
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

    public static class ILoadResultExtensions
    {
        public static ICompositeLoadResult ToParallel( this ILoadResult[] self )
        {
            return new ParallelLoadResult( self );
        }
        public static ICompositeLoadResult ToJoin( this ILoadResult self, Func<ILoadResult> onNext )
        {
            return new JoinLoadResult( self, onNext );
        }
        public static IDisposable          OnComplete( this ILoadResult self, Action onCompleted )
        {
            self.OnCompleted = onCompleted;
            return self;
        }
    }
}

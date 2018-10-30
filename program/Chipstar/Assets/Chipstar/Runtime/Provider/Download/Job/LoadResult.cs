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
        Action OnCompleted { set; }
    }
    public interface ILoadResult<T> : ILoadResult
    {
        T Content { get; }
    }

    public sealed class LoadResult<T> : ILoadResult, ILoadResult<T>
    {
        //=====================================
        //  変数
        //=====================================
        private IDisposable         m_dispose       = null;
        private ILoadJob<T>         m_job           = null;

        //=====================================
        //  プロパティ
        //=====================================
        public Action   OnCompleted { private get; set; }
        public T        Content     { get; private set; }

        //=====================================
        //  関数
        //=====================================

        public LoadResult(
            ILoadJob<T>             job, 
            Action<T>    onCompleted, 
            IDisposable             dispose 
        )
        {
            m_job           = job;
            m_job.OnLoaded  = () =>
            {
                Content = m_job.Content;
                onCompleted( Content );
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
    public class JoinLoadResult<T> : ILoadResult<T>
    {
        //================================
        //  変数
        //================================
        private ILoadResult     m_prev = null;
        private ILoadResult<T>  m_next = null;

        //================================
        //  プロパティ
        //================================
        public Action   OnCompleted { set; private get; }
        public T        Content     { get; private set; }

        //================================
        //  関数
        //================================

        public JoinLoadResult( ILoadResult prev, Func<ILoadResult<T>> onNext )
        {
            m_prev = prev;
            m_prev.OnCompleted = () =>
            {
                m_next = onNext();
                m_next.OnCompleted = () =>
                {
                    Content = m_next.Content;
                    OnCompleted();
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
                Content = default(T);
            }
            OnCompleted = null;
        }
    }
    public class JoinLoadResult : JoinLoadResult<Empty>
    {
        //================================
        //  変数
        //================================
        public JoinLoadResult(
            ILoadResult prev, Func<ILoadResult> onNext ) 
            : base(prev, onNext)
        {
        }
    }

    public sealed class ParallelLoadResult : ILoadResult
    {
     //================================
        //  変数
        //================================
        private ILoadResult[] m_list     = null;
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
        public static ILoadResult ToParallel ( this ILoadResult[] self )
     {
            return new ParallelLoadResult( self );
        }
        public static ILoadResult<T> ToJoin<T>( this ILoadResult self, Func<ILoadResult<T>> onNext )
        {
            return new JoinLoadResult<T>( self, onNext );
        }
        public static ILoadResult<Empty> ToJoin(this ILoadResult self, Func<ILoadResult> onNext)
        {
            return new JoinLoadResult<Empty>(self, onNext);
        }
        public static IDisposable          OnComplete( this ILoadResult self, Action onCompleted )
        {
            self.OnCompleted = onCompleted;
            return self;
        }
    }
}

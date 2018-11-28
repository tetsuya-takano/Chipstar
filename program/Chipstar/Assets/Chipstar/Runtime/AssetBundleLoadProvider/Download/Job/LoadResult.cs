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
        public bool     IsCompleted { get; private set; }

        //=====================================
        //  関数
        //=====================================

        public LoadResult(
            ILoadJob<T>		job, 
            Action<T>		onCompleted, 
            IDisposable     dispose 
        )
        {
            m_job           = job;
            m_job.OnLoaded  = () =>
            {
                IsCompleted = true;
                Content     = m_job.Content;
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
			if( m_dispose == null )
			{
				m_dispose.Dispose();
			}
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
        public bool     IsCompleted { get; private set; }
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
                    IsCompleted = true;
                    Content     = m_next.Content;
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
                Content = default(T);
            }
            OnCompleted = null;
        }
    }
    public sealed class ParallelLoadResult<T> : ILoadResult<IList<T>>
    {
     //================================
        //  変数
        //================================
        private ILoadResult<T>[] m_list  = null;
        private T[]              m_datas = null;
        private int              m_compCount   = 0;
        //================================
        //  プロパティ
        //================================
        public bool     IsCompleted { private set; get; }
        public Action   OnCompleted { set; private get; }
        public IList<T> Content     { get { return m_datas; } }

        //================================
        //  関数
        //================================

        public ParallelLoadResult( ILoadResult<T>[] args )
        {
            m_list      = args;
            m_datas     = new T[ m_list.Length ];
            m_compCount = 0;
            Action<T, ParallelLoadResult<T>> onDoneCallback = (c, result) =>
            {
                var i = result.m_compCount;
                result.m_datas[ i ] = c;
                result.m_compCount++;
                if( result.m_compCount < result.m_list.Length )
                {
                    return;
                }
                IsCompleted = true;
                result.OnCompleted();
            };

            foreach( var ret in m_list )
            {
                ret.OnCompleted = () => onDoneCallback(ret.Content, this);
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
            m_datas     = null;
            OnCompleted = null;
        }
    }

    public static class ILoadResultExtensions
    {
        public static ILoadResult<IList<T>> ToParallel<T>(this ILoadResult<T>[] self)
        {
            return new ParallelLoadResult<T>( self );
        }
        
        public static ILoadResult<TNext> ToJoin<TNext>( this ILoadResult self, Func<ILoadResult<TNext>> onNext )
        {
            return new JoinLoadResult<TNext>( self, onNext );
        }
        public static ILoadResult AsEmpty<T>( this ILoadResult<T> self )
        {
            return self;
        }
        public static IDisposable OnComplete( this ILoadResult self, Action onCompleted )
        {
            self.OnCompleted = onCompleted;
            return self;
        }
        public static IDisposable OnComplete<T>(this ILoadResult<T> self, Action<T> onCompleted )
        {
            self.OnCompleted = () => onCompleted( self.Content );
            return self;
        }
    }
}

using System;
using System.Collections.Generic;

namespace Chipstar.Downloads
{
    public interface IDownloadEngine
    {
        void Update();
        ILoadTask Enqueue( ILoadRequest bundle );
    }

    public class DownloadEngine : IDownloadEngine
    {
        //================================
        //  
        //================================
        private Queue<ILoadJob> m_queue     = new Queue<ILoadJob>();
        private ILoadJob        m_current   = null;

        //================================
        //  更新
        //================================

        public void Update()
        {
            //  現在のリクエストを処理
            if ( m_current != null && !m_current.IsCompleted )
            {
                m_current.Update();
                if( m_current.IsCompleted )
                {
                    m_current.Done();
                    m_current = null;
                    return;
                }
            }

            //  次のリクエストを処理
            if( m_queue.Count <= 0 )
            {
                return;
            }
            m_current = m_queue.Dequeue();
        }

        /// <summary>
        /// 追加
        /// </summary>
        public virtual ILoadTask Enqueue( ILoadRequest req )
        {
            var job = CreateJob( req );
            m_queue.Enqueue( job );

            return job;
        }

        protected virtual ILoadJob CreateJob( ILoadRequest req )
        {
            return null;
        }
    }
}
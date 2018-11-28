using System;
using System.Collections.Generic;

namespace Chipstar.Downloads
{
    public interface IJobEngine : IDisposable
    {
        void Update();
        void Enqueue(ILoadJob request);
    }

    //================================
    //  DL用のリクエストを積んで順次処理する
    //================================
    public class JobEngine : IJobEngine
    {
        //================================
        //  
        //================================
        private Queue<ILoadJob> m_queue     = new Queue<ILoadJob>();
        private ILoadJob        m_current   = null;

        //================================
        //  更新
        //================================

        public void Dispose()
        {
            if (m_current != null )
            {
                m_current.Dispose();
            }
            m_current = null;
            m_queue.Clear();
        }

        public void Update()
        {
            //  現在のリクエストを処理
            if (UpdateCurrent())
            {
                //  実行中
                return;
            }
            //  次のリクエストを処理
            MoveNext();
        }

        protected virtual bool UpdateCurrent()
        {
            if (m_current == null)
            {
                return false;
            }
            m_current.Update();
            if (!m_current.IsCompleted )
            {
                return true;
            }
            m_current.Done();
            m_current.Dispose();
            m_current = null;
			//	ここまで止めて次のジョブは次のフレームに回す
            return true;
        }

        protected virtual void MoveNext()
        {
            if (m_queue.Count <= 0)
            {
                return;
            }
            m_current = m_queue.Dequeue();
            m_current.Run();
        }

        /// <summary>
        /// 追加
        /// </summary>
        public virtual void Enqueue(ILoadJob job)
        {
            m_queue.Enqueue( job );
        }
    }
}
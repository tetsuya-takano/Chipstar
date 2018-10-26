using System.Collections.Generic;

namespace Chipstar.Downloads
{
    internal interface IDownloadEngine
    {
        void Update();
    }

    public class DownloadEngine : IDownloadEngine
    {
        private Queue<ILoadRequest> m_queue     = new Queue<ILoadRequest>();
        private ILoadRequest        m_current   = null;


        public void Update()
        {
            if ( m_current != null  && !m_current.IsCompleted )
            {
                m_current.Update();
                return;
            }
            if (m_current.IsCompleted )
            {
                m_current.Done( );
                m_current = m_queue.Dequeue();

                return;
            }

        }
    }
}
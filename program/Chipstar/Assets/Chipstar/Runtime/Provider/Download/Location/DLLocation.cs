using UnityEngine;
using System.Collections;
using System;

namespace Chipstar.Downloads
{
    public interface IDLLocation : IDisposable
    {
        
    }
    public abstract class DLLocation : IDLLocation
    {
        private bool m_isDisposed = false;
        public void Dispose()
        {
            if (m_isDisposed) { return; }
            DoDispose();
            m_isDisposed = true;
        }

        protected virtual void DoDispose() { }
    }
}
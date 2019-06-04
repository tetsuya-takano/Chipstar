using System;

namespace Chipstar.Downloads
{
	public interface IAccessLocation : IDisposable
	{
		string AccessKey	{ get; }
        string FullPath		{ get; }   
    }
    public abstract class DLLocation : IAccessLocation
    {
        //===================================
        //  変数
        //===================================
        private bool m_isDisposed = false;

		//===================================
		//  プロパティ
		//===================================
		public abstract string AccessKey	{ get; }
		public abstract string FullPath		{ get; }
        
        //===================================
        //  関数
        //===================================
        public void Dispose()
        {
            if (m_isDisposed) { return; }
            DoDispose();
            m_isDisposed = true;
        }

        protected virtual void DoDispose() { }
    }
}
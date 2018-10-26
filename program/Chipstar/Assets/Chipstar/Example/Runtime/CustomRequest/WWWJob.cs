using UnityEngine;
using System.Collections;
using System;

namespace Chipstar.Downloads
{
    /// <summary>
    /// 
    /// </summary>
    public class WWWDLJob<THandler,T> 
        : ILoadJob
        where THandler : WWWDL.WWWHandler<T>, new()
    {
        //===============================
        //  プロパティ
        //===============================
        public  float               Progress     { get; set; }
        public  bool                IsCompleted  { get; private set; }
        public  Action<T>           OnLoaded     { set { m_handler.OnLoaded = value; } }

        private string Url          { get; set; }

        //===============================
        //  変数
        //===============================
        private THandler    m_handler = null;
        private WWW         m_www     = null;

        //===============================
        //  関数
        //===============================

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public WWWDLJob( string url )
        {
            Url         = url;
            m_handler   = new THandler();
        }

        public void Run()
        {
            m_www = new WWW( Url );
        }

        public void Update()
        {
            Progress    = m_www.progress;
            IsCompleted = m_www.isDone;
        }

        public void Done()
        {
            m_handler.Complete( m_www );
        }

        public void Dispose() { }
    }
}
using UnityEngine;
using System;

namespace Chipstar.Downloads
{
    public interface IJobCreator : IDisposable
    {
		ILoadJob<byte[]> BytesLoad(IJobEngine engine, IAccessLocation location);
		ILoadJob<string> TextLoad(IJobEngine engine, IAccessLocation location);
		ILoadJob<Empty> FileDL(IJobEngine engine, IAccessLocation sourcePath, IAccessLocation localPath);
		ILoadJob<AssetBundle> OpenLocalBundle(IJobEngine engine, IAccessLocation location, Hash128 hash, uint crc);
	}
	public abstract class JobCreator : IJobCreator
    {
        //=======================================
        //  変数
        //=======================================

        //=======================================
        //  関数
        //=======================================

        /// <summary>
        /// 破棄処理
        /// </summary>
        public void Dispose()
        {
            DoDispose();
        }
        protected virtual void DoDispose() { }

        /// <summary>
        /// 生データ取得リクエスト
        /// </summary>
        public ILoadJob<byte[]> BytesLoad( IJobEngine engine, IAccessLocation location )
        {
            return AddJob( engine, DoCreateBytesLoad( location ) );
        }
        /// <summary>
        /// テキスト取得リクエスト
        /// </summary>
        public ILoadJob<string> TextLoad( IJobEngine engine, IAccessLocation location )
        {
            return AddJob( engine, DoCreateTextLoad( location ) );
        }
		/// <summary>
		/// アセットバンドルDL
		/// </summary>
		public ILoadJob<Empty> FileDL(IJobEngine engine, IAccessLocation source, IAccessLocation local)
		{
			return AddJob<Empty>(engine, DoCreateFileDL(source, local));
		}
		/// <summary>
		/// ローカルアセットバンドルオープン
		/// </summary>
		public ILoadJob<AssetBundle> OpenLocalBundle( IJobEngine engine, IAccessLocation location, Hash128 hash, uint crc )
		{
			return AddJob( engine, DoCreateLocalLoad( location, hash, crc ));
		}

        protected virtual ILoadJob<T> AddJob<T>( IJobEngine engine, ILoadJob<T> job )
        {
            engine.Enqueue( job );
            return job;
        }

		protected abstract ILoadJob<byte[]> DoCreateBytesLoad(IAccessLocation location);
		protected abstract ILoadJob<string> DoCreateTextLoad(IAccessLocation location);
		protected abstract ILoadJob<Empty> DoCreateFileDL(IAccessLocation source, IAccessLocation local);
		protected abstract ILoadJob<AssetBundle> DoCreateLocalLoad(IAccessLocation location, Hash128 hash, uint crc);
	}
}
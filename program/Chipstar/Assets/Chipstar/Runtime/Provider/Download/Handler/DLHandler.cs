using UnityEngine;
using System.Collections;
using System;

namespace Chipstar.Downloads
{
    public interface IDLHandler<TSource, TData> : IDisposable
    {
        Action<TData> OnLoaded { set; }
        void Complete( TSource source );
    }
    public abstract class DLHandler<TSource, TData>
        : IDLHandler<TSource, TData>
    {
        public Action<TData> OnLoaded { set; protected get; }
        public virtual void Complete(TSource source)
        {
            if (OnLoaded == null) { return; }
            DoComplete( source );
            OnLoaded = null;
        }
        protected abstract void DoComplete(TSource source);

        public void Dispose()
        {
            OnLoaded = null;
        }
    }
}
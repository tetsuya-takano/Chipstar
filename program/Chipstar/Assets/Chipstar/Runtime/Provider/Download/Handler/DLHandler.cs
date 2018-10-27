using UnityEngine;
using System.Collections;
using System;

namespace Chipstar.Downloads
{
    public interface IDLHandler<TSource, TData> : IDisposable
    {
        TData Complete( TSource source );
    }
    public abstract class DLHandler<TSource, TData>
        : IDLHandler<TSource, TData>
    {
        public virtual TData Complete(TSource source)
        {
            return DoComplete( source );
        }
        protected abstract TData DoComplete(TSource source);

        public void Dispose()
        {
        }
    }
}
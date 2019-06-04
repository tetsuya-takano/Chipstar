using System;

namespace Chipstar.Downloads
{
    public interface ILoadStatus : IDisposable
    {
        float       Progress    { get; }
        bool        IsCompleted { get; }
		bool		IsError		{ get; }
	}

	public interface ILoadStatus<T> : ILoadStatus
    {
        T Content { get; }
    }
}

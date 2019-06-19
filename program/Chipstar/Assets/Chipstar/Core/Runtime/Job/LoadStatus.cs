using System;

namespace Chipstar.Downloads
{
	public interface ILoadStatus : IDisposable
	{
		float Progress { get; }
		bool IsRunning { get; }
		bool IsCompleted { get; }
		bool IsFinish { get; }
		bool IsError { get; }
		bool IsCanceled { get; }
		bool IsDisposed { get; }
	}

	public interface ILoadStatus<T> : ILoadStatus
	{
		T Content { get; }
	}
}

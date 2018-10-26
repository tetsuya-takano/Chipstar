using System;

namespace Chipstar.Downloads
{
    public interface ILoadJob : ILoadTask, ILoadRequest
    {
        void Update();
        void Done();
    }

}
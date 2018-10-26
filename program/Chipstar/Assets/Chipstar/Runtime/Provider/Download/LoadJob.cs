using System;

namespace Chipstar.Downloads
{
    public interface ILoadJob : ILoadTask
    {
        void Run();
        void Update();
        void Done();
    }

}
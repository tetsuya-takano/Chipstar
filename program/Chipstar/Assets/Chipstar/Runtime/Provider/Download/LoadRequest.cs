using UnityEngine;
using System.Collections;

namespace Chipstar.Downloads
{
    public interface ILoadRequest
    {
        string Url { get; }

    }

    public interface IABRequest : ILoadRequest
    {
        void Done();

    }
}
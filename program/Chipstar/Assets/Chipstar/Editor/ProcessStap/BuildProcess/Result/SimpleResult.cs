using UnityEngine;
using UnityEditor;
namespace Chipstar.Builder
{

    public class SimpleResult : IABBuildResult
    {
        public bool IsSuccess { get; set; }

        public SimpleResult( bool success )
        {
            IsSuccess = success;
        }
    }
}
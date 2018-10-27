using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chipstar.Downloads
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class CompositeTask<T>
        : ILoadTask
        where T : ILoadTask
    {
        public bool IsCompleted
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public float Progress
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// 破棄処理
        /// </summary>
        public void Dispose()
        {
        }
    }
}

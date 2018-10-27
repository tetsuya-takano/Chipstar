using System.Collections.Generic;

namespace Chipstar.Downloads
{
    public interface ILoadDatabase<T> where T : IRuntimeBundleData
    {
        T Find( string path );
    }

    public class LoadDatabase<T> : ILoadDatabase<T> where T : IRuntimeBundleData
    {
        private Dictionary<string, T> m_table = null;
        public T Find(string path)
        {
            return m_table[ path ];
        }
    }
}
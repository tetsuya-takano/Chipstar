using UnityEngine;
using System.Collections;
using System.IO;

namespace Chipstar.Downloads
{
    public interface IEntryPoint
    {
        string BasePath { get; }
        IAccessLocation ToLocation( string path );
        IAccessLocation ToLocation<TRuntimeData>(TRuntimeData data) where TRuntimeData : IRuntimeBundleData<TRuntimeData>;
    }
    public class EntryPoint : IEntryPoint
    {
        //==================================
        //  プロパティ
        //==================================
        public string BasePath { get; private set; }

        //==================================
        //  関数
        //==================================

        public EntryPoint( string path )
        {
            BasePath = path;
        }

        public IAccessLocation ToLocation( string fileName )
        {
            return new UrlLocation( ToAccessPath( fileName ));
        }

        public IAccessLocation ToLocation<TRuntimeData>(TRuntimeData data) where TRuntimeData : IRuntimeBundleData<TRuntimeData>
        {
            return new UrlLocation( ToAccessPath( data.Name ));
        }

        protected virtual string ToAccessPath( string file )
        {
            var result = Path.Combine( BasePath, file );
            return result;
        }
    }
}
using UnityEngine;
using System.Collections;
using System.IO;

namespace Chipstar.Downloads
{
    public interface IAccessPoint
    {
        string BasePath { get; }
        IAccessLocation ToLocation( string path );
        IAccessLocation ToLocation<TRuntimeData>(TRuntimeData data) where TRuntimeData : IRuntimeBundleData<TRuntimeData>;
    }
    public class AccessPoint : IAccessPoint
    {
        //==================================
        //  プロパティ
        //==================================
        public string BasePath { get; private set; }

        //==================================
        //  関数
        //==================================

        public AccessPoint( string path )
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
            Debug.Log( result );
            return result;
        }
    }
}
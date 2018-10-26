using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
namespace Chipstar.Builder
{
    public interface IABBuildFileFilter
    {
        string[] Refine( string[] allAssetPaths );
    }


    public class ABBuildFileFilter : IABBuildFileFilter
    {
        public static readonly ABBuildFileFilter Empty = new ABBuildFileFilter( null );

        protected virtual string[] Extensions { get; private set; }


        public ABBuildFileFilter( 
            string[] ignoreExtensions
        )
        {
            Extensions = ignoreExtensions;
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual string[] Refine( string[] allAssetPaths )
        {
            return allAssetPaths
                    .Where( p => IsInProject( p ))
                    .Where( p => !IsFolder  ( p ) )
                    .Where( p => !IsMatchExtensions( p ) )
                    .ToArray();
        }

        protected bool IsInProject( string path )
        {
            return path.StartsWith( "Assets/" );
        }


        protected bool IsFolder( string path )
        {
            return !Path.HasExtension( path );
        }

        /// <summary>
        /// 拡張子チェック
        /// </summary>
        protected bool IsMatchExtensions( string path )
        {
            if( !Path.HasExtension( path ) )
            {
                return false;
            }
            if( Extensions == null || Extensions.Length == 0)
            {
                return false;
            }
            var extension = Path.GetExtension( path );
            foreach( var ignore in Extensions)
            {
                if( ignore == extension )
                {
                    return true;
                }
            }
            return false;
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
namespace Chipstar.Builder
{
    public interface IABBuildFileFilter
    {
        string[] Refine( string[] allAssetPaths );
    }

	/// <summary>
	/// 対象ファイルフィルタ
	/// </summary>
    public class ABBuildFileFilter : IABBuildFileFilter
    {
		//====================================
		//	変数
		//====================================
		private Regex[] m_regexes = null;
		//====================================
		//	変数
		//====================================

		public ABBuildFileFilter( 
            string[] ignorePattern
        )
        {
            if( ignorePattern == null )
			{
				return;
			}
			m_regexes = new Regex[ ignorePattern.Length ];
			for( int i = 0; i < ignorePattern.Length; i++ )
			{
				m_regexes[i] = new Regex( ignorePattern[ i ] );
			}
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual string[] Refine( string[] allAssetPaths )
        {
            return allAssetPaths
                    .Where( p => IsInProject( p ))
                    .Where( p => !IsFolder  ( p ) )
                    .Where( p => !IsMatchIgnore( p ) )
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
        /// 無視パターン判定
        /// </summary>
        protected bool IsMatchIgnore( string path )
        {
			if( m_regexes == null || m_regexes.Length == 0 )
			{
				return false;
			}

			foreach( var regex in m_regexes )
			{
				if( regex.IsMatch( path ) )
				{
					return true;
				}
			}
            return false;
        }
    }
}
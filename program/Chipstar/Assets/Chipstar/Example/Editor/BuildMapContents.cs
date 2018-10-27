using UnityEngine;
using UnityEditor;
using System;
using Chipstar.Builder;
using System.Collections.Generic;

namespace Chipstar.Example
{
    [Serializable]
    public struct BundleBuildData
    {
        //===============================
        //  変数
        //===============================
        [SerializeField] private string     m_abName        ;
        [SerializeField] private string[]   m_assets        ;
        [SerializeField] private string[]   m_dependencies  ;
        [SerializeField] private string     m_hash          ;

        //===============================
        //  関数
        //===============================
        public string   ABName
        {
            get { return m_abName; }
            set { m_abName = value; }
        }
        public string[] Assets
        {
            get { return m_assets; }
            set { m_assets = value; }
        }
        public string   Hash
        {
            get { return m_hash; }
            set { m_hash   = value; }
        }
        public string[] Dependencies
        {
            get { return m_dependencies; }
            set { m_dependencies = value; }
        }

        //===============================
        //  関数
        //===============================

        public BundleBuildData( 
            string      abName, 
            string[]    assets,
            string[]    dependenceis,
            string      hash
            )
        {
            m_abName        = abName;
            m_assets        = assets;
            m_hash          = hash;
            m_dependencies  = dependenceis;
        }
    }

    [Serializable]
    public struct AssetBuildData
    {
        [SerializeField] private string m_path;
        [SerializeField] private string m_guid;

        public string Path
        {
            get { return m_path; }
            set { m_path = value; }
        }
        public string Guid
        {
            get { return m_guid; }
            set { m_guid = value; }
        }
    }

    [Serializable]
    public class BuildMapContents
    {
        //===============================
        //  SerializeField
        //===============================
        [SerializeField] private  List<BundleBuildData> m_abManifestList = new List<BundleBuildData>();
        [SerializeField] private  List<AssetBuildData>  m_assetDBList    = new List<AssetBuildData>();

        //===============================
        //  関数
        //===============================

        public void AddBundle( BundleBuildData data )
        {
            m_abManifestList.Add( data );
        }

        public void AddAsset( AssetBuildData data )
        {
            m_assetDBList.Add( data );
        }
    }
}
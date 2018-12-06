using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Chipstar.Downloads
{
    public interface IBundleBuildData
    {
        string      ABName       { get; }
        string[]    Assets       { get; }
        string      Hash         { get; }
		uint		Crc			 { get; }
        string[]    Dependencies { get; }
    }
    public interface IAssetBuildData
    {
        string Path { get; }
        string Guid { get; }
    }
    public interface IBuildMapDataTable<TBundle, TAssetData>
        where TBundle   : IBundleBuildData
        where TAssetData: IAssetBuildData
    {
        IEnumerable<TBundle>    BundleList  { get; }
        IEnumerable<TAssetData> AssetList   { get; }
		string					Prefix		{ get; }

		void Add(TAssetData asset  );
        void Add(TBundle    bundle );
    }

    [Serializable]
    public struct BundleBuildData : IBundleBuildData
    {
        //===============================
        //  変数
        //===============================
        [SerializeField] private string m_abName;
        [SerializeField] private string[] m_assets;
        [SerializeField] private string[] m_dependencies;
        [SerializeField] private string m_hash;
		[SerializeField] private uint   m_crc;
        [SerializeField] private long   m_fileSize;

        //===============================
        //  関数
        //===============================
        public string ABName
        {
            get { return m_abName; }
            set { m_abName = value; }
        }
        public string[] Assets
        {
            get { return m_assets; }
            set { m_assets = value; }
        }
        public string Hash
        {
            get { return m_hash; }
            set { m_hash = value; }
        }
        public string[] Dependencies
        {
            get { return m_dependencies; }
            set { m_dependencies = value; }
        }

        public long FileSize
        {
            get { return m_fileSize; }
            set { m_fileSize = value; }
        }

		public uint Crc
		{
			get { return m_crc; }
			set { m_crc = value; }
		}

		//===============================
		//  関数
		//===============================
		public BundleBuildData(
            string  abName,
            string[]assets,
            string[]dependenceis,
            string  hash,
			uint	crc,
            long    size
            )
        {
            m_abName        = abName;
            m_assets        = assets;
            m_hash          = hash;
			m_crc           = crc;
            m_dependencies  = dependenceis;
            m_fileSize      = size;
        }
    }

    [Serializable]
    public struct AssetBuildData : IAssetBuildData
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
    public class BuildMapDataTable : IBuildMapDataTable<BundleBuildData, AssetBuildData>
    {
        //===============================
        //  SerializeField
        //===============================
		[SerializeField] private string					m_prefix		= string.Empty;
        [SerializeField] private List<BundleBuildData>  m_bundleList	= new List<BundleBuildData>();
        [SerializeField] private List<AssetBuildData>   m_assetDBList	= new List<AssetBuildData>();

		//===============================
		//  プロパティ
		//===============================
		public string Prefix
		{
			get { return m_prefix; }
			set { m_prefix = value; }
		}
		public IEnumerable<BundleBuildData> BundleList { get { return m_bundleList; } }
		public IEnumerable<AssetBuildData> AssetList { get { return m_assetDBList; } }


		//===============================
		//  関数
		//===============================
		public BuildMapDataTable()
        {
            m_bundleList = new List<BundleBuildData>();
            m_assetDBList= new List<AssetBuildData>();
        }

        public void Add(BundleBuildData data)
        {
            m_bundleList.Add(data);
        }

        public void Add(AssetBuildData data)
        {
            m_assetDBList.Add(data);
        }
    }
}
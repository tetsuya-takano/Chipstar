using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using System.IO;
using System.Text;
using UnityEngine.Serialization;

namespace Chipstar.Downloads
{
    public interface IBundleBuildData
    {
        string      ABName       { get; }
        string[]    Assets       { get; }
        string      Hash         { get; }
		uint		Crc			 { get; }
		long		FileSize	 { get; }
        string[]    Dependencies { get; }
		string[]	Labels		 { get; }
		string		AssetVersion { get; }
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
        [SerializeField] private string		m_abName;
        [SerializeField] private string[]	m_assets;
        [SerializeField] private string[]	m_dependencies;
		[SerializeField] private string		m_assetVersionHash;
        [SerializeField] private string		m_hash;
		[SerializeField] private uint		m_crc;
        [SerializeField] private long		m_fileSize;
		[SerializeField] private string[]   m_labels;

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

		public string[] Labels
		{
			get { return m_labels; }
			set { m_labels = value; }
		}

		public string AssetVersion
		{
			get { return m_assetVersionHash;  }
			set { m_assetVersionHash = value; }
		}

		//===============================
		//  関数
		//===============================
		public BundleBuildData(
            string		abName,
            string[]	assets,
            string[]	dependenceis,
            string		hash,
			uint		crc,
            long		size,
			string[]	labels,
			string		version
            )
        {
            m_abName         = abName;
            m_assets         = assets;
            m_hash= hash;
			m_crc            = crc;
            m_dependencies   = dependenceis;
            m_fileSize       = size;
			m_labels         = labels;
			m_assetVersionHash=version;
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
	public class BuildMapDataTable : IBuildMapDataTable<BundleBuildData, AssetBuildData>, ISerializationCallbackReceiver
	{
		//===============================
		//  const
		//===============================
		private static readonly Encoding Encode = new UTF8Encoding( false );

		//===============================
		//  SerializeField
		//===============================
		[SerializeField] private string					m_prefix		= string.Empty;
        [SerializeField] private List<BundleBuildData>  m_bundleList	= new List<BundleBuildData>();
        [SerializeField] private List<AssetBuildData>   m_assetDBList	= new List<AssetBuildData>();

		//===============================
		//  変数
		//===============================
		[NonSerialized] private Dictionary<string, BundleBuildData> m_runtimeBundleTable = new Dictionary<string, BundleBuildData>();
		[NonSerialized] private Dictionary<string, AssetBuildData>  m_runtimeAssetTable  = new Dictionary<string, AssetBuildData>();

		//===============================
		//  プロパティ
		//===============================
		public string Prefix
		{
			get { return m_prefix; }
			set { m_prefix = value; }
		}
		public IEnumerable<BundleBuildData> BundleList { get { return m_runtimeBundleTable.Values; } }
		public IEnumerable<AssetBuildData>  AssetList  { get { return m_runtimeAssetTable .Values; } }


		//===============================
		//  関数
		//===============================

		public static BuildMapDataTable Read( string saveFilePath )
		{
			if( !File.Exists( saveFilePath ))
			{
				return new BuildMapDataTable();
			}
			var json = File.ReadAllText( saveFilePath, Encode );
			return JsonUtility.FromJson<BuildMapDataTable>( json );
		}

		public static bool Write( string saveFilePath, BuildMapDataTable table )
		{
			var contents    = JsonUtility.ToJson( table, true );
			if( string.IsNullOrWhiteSpace( contents))
			{
				return false;
			}
			//  書き込み
			File.WriteAllText( saveFilePath, contents, Encode );

			return true;
		}

		public BuildMapDataTable()
        {
            m_bundleList = new List<BundleBuildData>();
            m_assetDBList= new List<AssetBuildData>();
			m_runtimeAssetTable = new Dictionary<string, AssetBuildData>();
			m_runtimeBundleTable= new Dictionary<string, BundleBuildData>();
        }

        public void Add(BundleBuildData data)
        {
			if( m_runtimeBundleTable.ContainsKey( data.ABName))
			{
				m_runtimeBundleTable[ data.ABName ] = data;
				return;
			}
			m_runtimeBundleTable.Add( data.ABName, data );
        }
        public void Add(AssetBuildData data)
        {
			if( m_runtimeAssetTable.ContainsKey( data.Path ) )
			{
				m_runtimeAssetTable[ data.Path ] = data;
				return;
			}
			m_runtimeAssetTable.Add( data.Path, data );
		}

		public BundleBuildData GetBundle( string abName )
		{
			if( !m_runtimeBundleTable.ContainsKey( abName ))
			{
				return default( BundleBuildData );
			}
			return m_runtimeBundleTable[ abName ];
		}
		public AssetBuildData GetAsset( string path )
		{
			if( !m_runtimeAssetTable.ContainsKey( path) )
			{
				return default( AssetBuildData );
			}
			return m_runtimeAssetTable[path];
		}

		/// <summary>
		/// 
		/// </summary>
		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
			m_assetDBList = m_runtimeAssetTable.Values.ToList();
			m_runtimeAssetTable.Clear();

			m_bundleList = m_runtimeBundleTable.Values.ToList();
			m_runtimeBundleTable.Clear();
		}

		/// <summary>
		/// 
		/// </summary>
		void ISerializationCallbackReceiver.OnAfterDeserialize()
		{
			m_runtimeBundleTable = m_bundleList.ToDictionary( c => c.ABName );
			m_bundleList.Clear();

			m_runtimeAssetTable = m_assetDBList.ToDictionary( c => c.Path );
			m_assetDBList.Clear();
		}
	}
}
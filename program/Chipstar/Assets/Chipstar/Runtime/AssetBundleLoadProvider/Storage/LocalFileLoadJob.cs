using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Chipstar.Downloads
{
	/// <summary>
	/// ローカルのアセットバンドルを取りに行くジョブ
	/// </summary>
	public sealed class LocalFileLoadJob
		: LoadJob<LocalABHandler, AssetBundleCreateRequest, AssetBundle>
	{
		//===============================
		//	変数
		//===============================
		private uint m_crc = 0;

		//===============================
		//	関数
		//===============================

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public LocalFileLoadJob( IAccessLocation location, uint crc )
			: base( location, new LocalABHandler() ) 
		{
			m_crc = crc;
		}

		/// <summary>
		/// 開始
		/// </summary>
		protected override void DoRun( IAccessLocation location )
		{
			Source = AssetBundle.LoadFromFileAsync( location.AccessPath, m_crc );
		}

		protected override float DoGetProgress( AssetBundleCreateRequest source )
		{
			return source.progress;
		}

		protected override bool DoIsComplete( AssetBundleCreateRequest source )
		{
			return source.isDone;
		}

		protected override bool DoIsError( AssetBundleCreateRequest source )
		{
			return source.isDone && source.assetBundle == null;
		}
	}

	public sealed class LocalABHandler : ILoadJobHandler<AssetBundleCreateRequest, AssetBundle>
	{
		/// <summary>
		/// 
		/// </summary>
		public AssetBundle Complete( AssetBundleCreateRequest source )
		{
			return source.assetBundle;
		}

		/// <summary>
		/// 破棄処理
		/// </summary>
		public void Dispose()
		{
		}
	}
}

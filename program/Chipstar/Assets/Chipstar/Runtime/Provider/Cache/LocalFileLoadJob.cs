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
		//	関数
		//===============================

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public LocalFileLoadJob( IAccessLocation location )
			: base( location, new LocalABHandler() ) 
		{

		}

		/// <summary>
		/// 開始
		/// </summary>
		protected override void DoRun( IAccessLocation location )
		{
			Source = AssetBundle.LoadFromFileAsync( location.AccessPath );
		}

		/// <summary>
		/// 更新
		/// </summary>
		protected override void DoUpdate( AssetBundleCreateRequest source )
		{
			Progress	= source.progress;
			IsCompleted = source.isDone;
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

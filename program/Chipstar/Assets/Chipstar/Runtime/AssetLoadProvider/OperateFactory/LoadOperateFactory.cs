using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chipstar.AssetLoad
{
	/// <summary>
	/// 読み込みタスクを作成する
	/// コレはただのマーカー
	/// </summary>
	public interface ILoadOperateFactory : IDisposable
	{
		bool CanLoad( string path );
	}

	/// <summary>
	/// アセット読み込みをするヤツ
	/// </summary>
	public interface IAssetLoadFactory : ILoadOperateFactory
	{
		ILoadOperation<T> Create<T>( string path ) where T : UnityEngine.Object;
	}

	/// <summary>
	/// シーン読み込みをするヤツ
	/// </summary>
	public interface ISceneLoadFactory : ILoadOperateFactory
	{
		AsyncOperation LoadLevel		( string path );
		AsyncOperation LoadLevelAdditive( string path );
	}
}
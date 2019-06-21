using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Chipstar.Downloads
{
	/// <summary>
	/// 読み込みタスクを作成する
	/// コレはただのマーカー
	/// </summary>
	public interface ILoadOperateFactory : IDisposable
	{
		int Priority { get; }
		bool CanLoad( string path );
	}

	/// <summary>
	/// アセット読み込みをするヤツ
	/// </summary>
	public interface IAssetLoadFactory : ILoadOperateFactory
	{
		IAssetLoadOperater<T> Create<T>( string path ) where T : UnityEngine.Object;
	}

	/// <summary>
	/// シーン読み込みをするヤツ
	/// </summary>
	public interface ISceneLoadFactory : ILoadOperateFactory
	{
		ISceneLoadOperater Create(string path, LoadSceneMode mode);
	}
}
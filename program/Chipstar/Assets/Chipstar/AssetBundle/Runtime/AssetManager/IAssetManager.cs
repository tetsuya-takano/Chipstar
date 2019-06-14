using Chipstar.Downloads;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Chipstar
{
	/// <summary>
	/// リソース管理機能の統合インターフェイス
	/// </summary>
	public interface IAssetManager<TBundleData> : IDisposable
		where TBundleData : IRuntimeBundleData<TBundleData>
	{
		/// <summary>
		/// 初期化
		/// </summary>
		IEnumerator Setup();

		/// <summary>
		/// リモートデータの取得
		/// </summary>
		IEnumerator Login( IAccessPoint server );

		/// <summary>
		/// リモートデータの破棄
		/// </summary>
		void Logout();

		/// <summary>
		/// ダウンロード
		/// </summary>
		IPreloadOperation DeepDownload( string assetPath );

		/// <summary>
		/// ダウンロード
		/// </summary>
		IPreloadOperation SingleDownload( string abName );

		/// <summary>
		/// アセットバンドルオープン
		/// </summary>
		IPreloadOperation DeepOpenFile( string assetPath );
		IPreloadOperation SingleOpenFile( string abName );

		/// <summary>
		/// アセットの読み込み
		/// </summary>
		IAssetLoadOperation<T>	LoadAsset<T>( string assetPath ) where T : UnityEngine.Object;

		/// <summary>
		/// シーン遷移
		/// </summary>
		ISceneLoadOperation LoadLevel( string scenePath, LoadSceneMode mode );

		/// <summary>
		/// アセットの破棄
		/// </summary>
		IEnumerator Unload( bool isForceUnloadAll );

		/// <summary>
		/// 保存データの破棄(キャッシュクリア)
		/// </summary>
		IEnumerator StorageClear();

		/// <summary>
		/// 更新処理
		/// </summary>
		void DoUpdate();

		/// <summary>
		/// 後処理
		/// </summary>
		void DoLateUpdate();

		/// <summary>
		/// ファイルの検索
		/// </summary>
		IEnumerable<string> SearchFileList( string searchKey );

		/// <summary>
		/// DLの必要なファイル情報を取得
		/// </summary>
		IEnumerable<TBundleData> GetNeedDownloadList();

		/// <summary>
		/// 全バンドル情報
		/// </summary>
		IEnumerable<TBundleData> GetList();

		/// <summary>
		/// キャッシュ済み判定
		/// </summary>
		bool IsCache( string abName );

		/// <summary>
		/// 保存ディレクトリ
		/// </summary>
		IAccessPoint GetLocalDir( );

		/// <summary>
		/// ロードの停止
		/// </summary>
		void Stop();
	}
}
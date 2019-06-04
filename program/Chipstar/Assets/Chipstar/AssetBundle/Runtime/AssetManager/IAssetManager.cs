using Chipstar.Downloads;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
		IEnumerator	Setup();

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
		ILoadProcess DeepDownload( string assetPath );

		/// <summary>
		/// ダウンロード
		/// </summary>
		ILoadProcess SingleDownload( string abName );

		/// <summary>
		/// アセットバンドルオープン
		/// </summary>
		ILoadProcess DeepOpenFile( string assetPath );
		ILoadProcess SingleOpenFile( string abName );

		/// <summary>
		/// アセットの読み込み
		/// </summary>
		IAssetLoadOperation<T>	LoadAsset<T>( string assetPath ) where T : UnityEngine.Object;

		/// <summary>
		/// シーン遷移
		/// </summary>
		ISceneLoadOperation LoadLevel( string scenePath );

		/// <summary>
		/// シーン加算
		/// </summary>
		ISceneLoadOperation LoadLevelAdditive( string scenePath );

		/// <summary>
		/// 参照の解放
		/// </summary>
		void Release( string assetPath );

		/// <summary>
		/// 参照カウンタ作成
		/// </summary
		IDisposable CreateAssetReference( string assetPath );

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
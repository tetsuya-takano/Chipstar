using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Chipstar.Downloads
{
	/// <summary>
	/// アセットバンドルを取ってくる機能
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public sealed class AssetBundleLoadOperation<T,TBundleData> : AssetLoadOperation<T>
		where T : UnityEngine.Object
		where TBundleData : IRuntimeBundleData<TBundleData>
	{
		//====================================
		//	変数
		//====================================
		private AssetBundleRequest m_request = null;
		private AssetData<TBundleData> m_data = null;
		//====================================
		//	プロパティ
		//====================================
		//====================================
		//	関数
		//====================================

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public AssetBundleLoadOperation( AssetData<TBundleData> data )
		{
			m_data = data;
			m_data?.AddRef();
		}

		/// <summary>
		/// 破棄処理
		/// </summary>
		protected override void DoDispose()
		{
			m_data?.ReleaseRef();
			m_request = null;
			m_data = null;
			base.DoDispose();
		}
		/// <summary>
		/// 実行
		/// </summary>
		protected override void DoRun()
		{
			m_request = m_data?.LoadAsync<T>();
			ChipstarLog.AssertNotNull(m_request, $"Bundle Rquest is Null ::{m_data?.Path ?? string.Empty}");
		}

		protected override void DoComplete()
		{
			base.DoComplete();
		}

		protected override ResultCode DoError(Exception e)
		{
			return ChipstarResult.ClientError($"AssetBundle Assets Load Error :: {m_data?.Path}", e);
		}

		protected override float GetProgress()
		{
			return m_request.progress;
		}

		protected override bool GetComplete()
		{
			return m_request.isDone;
		}

		protected override T GetContent()
		{
			return m_request.asset as T;
		}

		public override string ToString()
		{
			return "[AssetBundleLoadOperation]" + (m_data?.Path ?? string.Empty);
		}
	}
}
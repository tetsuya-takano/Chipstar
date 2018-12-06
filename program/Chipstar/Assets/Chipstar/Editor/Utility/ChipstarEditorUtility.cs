using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Chipstar.Builder
{
	/// <summary>
	/// エディタ汎用機能
	/// </summary>
	public static class ChipstarEditorUtility
	{
		
	}

	/// <summary>
	/// 進捗ダイアログスコープ
	/// </summary>
	public sealed class ProgressDialogScope : IDisposable
	{
		//==============================
		//	変数
		//==============================
		private string	m_title = "";
		private int     m_count = 0;

		//==============================
		//	関数
		//==============================

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ProgressDialogScope( string title, int count )
		{
			m_title = title;
			m_count = count;
		}

		/// <summary>
		/// 表示
		/// </summary>
		public void Show( string message, int current )
		{
			var progress = Mathf.InverseLerp( 0, m_count, current );
			EditorUtility.DisplayProgressBar( m_title, message, progress );
		}
		/// <summary>
		/// 破棄
		/// </summary>
		public void Dispose()
		{
			EditorUtility.ClearProgressBar();
		}
	}
}
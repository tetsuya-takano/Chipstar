using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Chipstar.Downloads
{

	/// <summary>
	/// Chipstar用のリザルトコードクラス
	/// </summary>
	public sealed partial class ChipstarResult
	{
		/// <summary>
		/// リザルトコード管理
		/// </summary>
		private static class Code
		{
			//===================================
			//	const
			//===================================

			public const long NetworkErrorStart = 100000;
			public const long ClientErrorStart  = 200000;


			public const long Success = -1;
			public const long Generic = 0;

			public const long Invalid = 999999;
		}

		public static ResultCode Generic { get; } = new ResultCode(Code.Generic, ErrorLevel.Error, "Error Generic");
		public static ResultCode NotImpl { get; } = new ResultCode(Code.Invalid, ErrorLevel.Error, "NotImpl Error");
		public static ResultCode None { get; } = new ResultCode(Code.Success, ErrorLevel.None, "None");

		public static ResultCode NetworkError(long responceCode, string message)
		{
			return new ResultCode( Code.NetworkErrorStart + responceCode, ErrorLevel.Error, message);
		}

		public static ResultCode HttpError(long responseCode, string message)
		{
			return new ResultCode(Code.NetworkErrorStart + responseCode, ErrorLevel.Error, message);
		}

		public static ResultCode ClientError( string log )
		{
			return new ResultCode( Code.ClientErrorStart, ErrorLevel.Error, log );
		}
		public static ResultCode ClientError(string log, Exception e)
		{
			return new ResultCode(Code.ClientErrorStart, ErrorLevel.Error, log + "\n" + e.Message);
		}
	}
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Chipstar.Downloads
{
	public enum ErrorLevel
	{
		None,
		Warning,
		Error,
	}
	public sealed partial class ChipstarResultCode
	{
		public int Code { get; }
		public ErrorLevel Level { get; }
		public string Message { get; }

		public ChipstarResultCode(int code, ErrorLevel level, string message)
		{
			Code = code;
			Level = level;
			Message = message;
		}
	}

	public sealed partial class ChipstarResultCode
	{
		public static ChipstarResultCode Generic { get; } = new ChipstarResultCode(0, ErrorLevel.Error, "Error Generic");
		public static ChipstarResultCode None { get; } = new ChipstarResultCode(-1, ErrorLevel.None, "None");
	}

}
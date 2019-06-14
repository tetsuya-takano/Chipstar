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
	public sealed partial class ResultCode
	{
		public long Code { get; }
		public ErrorLevel Level { get; }
		public string Message { get; }

		public ResultCode(long code, ErrorLevel level, string message)
		{
			Code = code;
			Level = level;
			Message = message;
		}

		public override string ToString()
		{
			return $"{Level}[Code={Code}]::{Message}";
		}
	}

	

}
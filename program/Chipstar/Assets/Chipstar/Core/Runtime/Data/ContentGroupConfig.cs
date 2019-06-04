using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chipstar.Downloads
{
	[Serializable]
	public sealed class ContentGroupConfig
	{
		//====================================
		//	変数
		//====================================
		public string IncludeDirPath;
		public string StorageDirPath;
		public string RemoteDirPath;
		public string RemoteFileName;
		public string LocalFileName;
	}
}
using Chipstar.Downloads.CriWare;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Chipstar.Builder.CriWare
{
	/// <summary>
	/// サウンドバージョンテーブルの作成
	/// </summary>
	public sealed class SoundVersionBuilder
	{
		[Serializable]
		private class SoundFileData : ISoundFileData
		{
			public string CueSheetName; //	キューシート名
			public string DirPath;      //	階層
			public string AcbHash;      //	Acbファイルのハッシュ値
			public string AwbHash;      //	Awbファイルのハッシュ値
			public long AcbSize;      //	Acbファイル容量
			public long AwbSize;      //	Acbファイル容量
			public string AssetVersion; //	このアセットのバージョン

			string ISoundFileData.CueSheetName { get { return CueSheetName; } }

			string ISoundFileData.AcbPath { get { return Path.Combine(DirPath, string.Format("{0}.acb", CueSheetName)).ToConvertDelimiter(); } }
			string ISoundFileData.AwbPath { get { return Path.Combine(DirPath, string.Format("{0}.awb", CueSheetName)).ToConvertDelimiter(); } }

			string ISoundFileData.AwbHash { get { return AwbHash; } }
			string ISoundFileData.AcbHash { get { return AcbHash; } }

			long ISoundFileData.AwbSize { get { return AwbSize; } }
			long ISoundFileData.AcbSize { get { return AcbSize; } }

			string ISoundFileData.AssetVersion { get { return AssetVersion; } }

			bool ISoundFileData.HasAwb()
			{
				return !string.IsNullOrEmpty(AwbHash);
			}
			bool ISoundFileData.IsInclude()
			{
				return true;
			}
		}

		public bool Build( string directoryPath, string fileName )
		{
			var builder = new FileHashDatabaseBuilder
				(
					folder: directoryPath,
					pattern: new string[]
					{
						"(.*?).awb$","(.*?).acb$",
					}
				);
			if (!Directory.Exists(directoryPath))
			{
				Directory.CreateDirectory(directoryPath);
			}
			var outputPath = Path.Combine(directoryPath, fileName );
			var fileHashList = builder.Build();
			var table = CreateTable( fileHashList );

			return SoundFileDatabase.Write( outputPath, table );
		}

		/// <summary>
		/// 作成
		/// </summary>
		private SoundFileDatabase CreateTable(
			Dictionary<string, FileHashDatabaseBuilder.FileHashData> fileHashList
			)
		{
			var table = new SoundFileDatabase();

			//	拡張子を外して、キューシート名の一覧に
			var cueSheetList = fileHashList
								.Keys
								.Select(c => Path.GetFileNameWithoutExtension(c))
								.Distinct()
								.ToArray();
			//	キューシート名のグループを作成
			var cueSheetGroup = fileHashList
									.GroupBy(c => Path.GetFileNameWithoutExtension(c.Key))
									.ToArray();

			//	キューシート名を使ってデータを作成する
			foreach (var cueSheetName in cueSheetList)
			{
				//	同じキューシートの要素
				var group = cueSheetGroup.FirstOrDefault(c => c.Key == cueSheetName);
				Debug.Assert(group.Count() >= 0);
				Debug.Assert(group.Count() < 3);

				//	acb/awbファイルを取得
				var acbFile = group.FirstOrDefault(c => c.Key.Contains("acb"));
				var awbFile = group.FirstOrDefault(c => c.Key.Contains("awb"));

				var dirPath = Path.GetDirectoryName(acbFile.Key);
				var data = _ToData( dirPath, cueSheetName, acbFile.Value, awbFile.Value );

				table.Add(dirPath, data);
			}

			return table;
		}

		private SoundFileData _ToData(
			string dirPath,
			string cueSheetName,
			FileHashDatabaseBuilder.FileHashData acbFile,
			FileHashDatabaseBuilder.FileHashData awbFile
		)
		{
			var hasAwb = awbFile != null;
			return new SoundFileData
			{
				CueSheetName = cueSheetName,
				AcbHash = acbFile.Hash,
				AcbSize = acbFile.FileInfo.Length,
				AwbHash = hasAwb ? awbFile.Hash : string.Empty,
				AwbSize = hasAwb ? awbFile.FileInfo.Length : 0,
				DirPath = dirPath,
				AssetVersion = string.Empty
			};
		}
	}
}
using Chipstar.Downloads.CriWare;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Chipstar.Builder.CriWare
{
	/// <summary>
	/// 動画バージョンテーブルの作成
	/// </summary>
	public sealed class MovieVersionBuilder
	{
		[Serializable]
		private class MovieFileData : IMovieFileData
		{
			public string Key;
			public string Path;
			public string Hash;
			public long Size;
			public string AssetVersion;

			string IMovieFileData.Key { get { return Key; } }
			string IMovieFileData.Hash { get { return Hash; } }
			string IMovieFileData.Path { get { return Path; } }
			long IMovieFileData.Size { get { return Size; } }
			string IMovieFileData.AssetVersion { get { return AssetVersion; } }

			bool IMovieFileData.IsInclude()
			{
				return true;
			}
		}
		public bool Build( string directoryPath, string fileName )
		{
			if (!Directory.Exists(directoryPath))
			{
				Directory.CreateDirectory(directoryPath);
			}

			var builder = new FileHashDatabaseBuilder
					(
						folder : directoryPath,
						pattern: new string[]
						{
						"(.*?).usm$",
						}
					);
			var outputPath   = Path.Combine(directoryPath, fileName);
			var fileHashList = builder.Build();
			var table        = CreateTable( fileHashList );

			return MovieFileDatabase.Write(outputPath, table);
		}
		/// <summary>
		/// 作成
		/// </summary>
		private MovieFileDatabase CreateTable(
			Dictionary<string, FileHashDatabaseBuilder.FileHashData> fileHashList
		)
		{
			var table = new MovieFileDatabase();

			foreach (var file in fileHashList)
			{
				var data = file.Value;
				var info = data.FileInfo;
				var path = data.Key;
				var key = path.Replace(info.Extension, string.Empty);
				var size = info.Length;
				var hash = data.Hash;
				// usmファイル情報を追加
				table.Add(new MovieFileData
				{
					Key = key,
					Size = size,
					Hash = hash,
					Path = path,
					AssetVersion = string.Empty
				});
			}

			return table;
		}
	}
}
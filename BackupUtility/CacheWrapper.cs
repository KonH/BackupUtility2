using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace BackupUtility {
	sealed class CacheWrapper : IDisposable {
		public Dictionary<string, CachedFileInfo> Data;

		readonly string _path;

		public CacheWrapper(string path) {
			_path = path;

			Data = TryLoadData(_path) ?? new Dictionary<string, CachedFileInfo>();
		}

		public void Dispose() {
			SaveData(Data, _path);
		}

		static Dictionary<string, CachedFileInfo> TryLoadData(string path) {
			if ( !File.Exists(path) ) {
				return null;
			}
			var content = File.ReadAllText(path);
			return JsonConvert.DeserializeObject<Dictionary<string, CachedFileInfo>>(content);
		}

		static void SaveData(Dictionary<string, CachedFileInfo> data, string path) {
			var content = JsonConvert.SerializeObject(data, Formatting.Indented);
			File.WriteAllText(path, content);
		}
	}
}

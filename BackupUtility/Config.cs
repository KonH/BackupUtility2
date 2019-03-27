using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace BackupUtility {
	public sealed class Config {
		public Dictionary<string, string> Pathes = new Dictionary<string, string>();

		public static Config FromFile(string path) {
			var content = File.ReadAllText(path);
			return JsonConvert.DeserializeObject<Config>(content);
		}
	}
}

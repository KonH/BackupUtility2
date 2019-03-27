using System.Collections.Generic;

namespace BackupUtility {
	sealed class CacheData {
		public Dictionary<string, CachedFileInfo> Infos = new Dictionary<string, CachedFileInfo>();
	}
}

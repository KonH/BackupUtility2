using System.IO;

namespace BackupUtility {
	public struct BackupTask {
		public FileInfo File;
		public string   RelativePath;
		public bool     IsRequired;
		public bool     IsSuccess;
	}
}

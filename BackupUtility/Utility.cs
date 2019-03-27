using System;

namespace BackupUtility {
	public sealed class Utility {
		readonly Config _config;

		public Utility(Config config) {
			_config = config;
		}

		public void Process() {
			Console.WriteLine($"[ BackupUtility: {_config.Pathes.Count} tasks ]");
			foreach ( var pair in _config.Pathes ) {
				var from = pair.Key;
				var to = pair.Value;
				Console.WriteLine();
				Console.WriteLine($"[ '{from}' => '{to}' ]");
				new Runner(from, to).Process();
			}
		}
	}
}

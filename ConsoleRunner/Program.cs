using BackupUtility;

namespace ConsoleRunner {
	static class Program {
		static void Main(string[] args) {
			var config = Config.FromFile("config.json");
			new Utility(config).Process();
		}
	}
}

using System;
using Microsoft.Extensions.Logging;

namespace BackupUtility {
	sealed class Utility {
		readonly ILoggerFactory _loggerFactory;
		readonly Config         _config;

		public Utility(ILoggerFactory loggerFactory, Config config) {
			_loggerFactory = loggerFactory;
			_config        = config;
		}

		public void Process() {
			var logger = _loggerFactory.CreateLogger<Utility>();
			var startTime = DateTime.Now;
			logger.LogInformation($"Started at {startTime}");
			logger.LogInformation($"{_config.Pathes.Count} tasks\n");
			foreach ( var pair in _config.Pathes ) {
				var from = pair.Key;
				var to = pair.Value;
				new Runner(_loggerFactory, from, to, _config.HistoryDepth).Process();
			}
			var endTime = DateTime.Now;
			logger.LogInformation($"Finished at {endTime} (elapsed: {endTime - startTime})");
		}
	}
}

using System;
using Microsoft.Extensions.Logging;

namespace BackupUtility {
	sealed class ProgressTracker {
		readonly ILogger _logger;

		readonly long _maxLength;

		long _curLength  = 0;
		int  _curPercent = 0;

		public ProgressTracker(ILoggerFactory loggerFactory, long maxLength) {
			_logger    = loggerFactory.CreateLogger<ProgressTracker>();
			_maxLength = maxLength;
		}

		public void UpdateProgress(long portion) {
			_curLength += portion;
			var newPercent = (int)Math.Round(((double)_curLength / _maxLength) * 100);
			if ( newPercent > _curPercent ) {
				_logger.LogInformation($"{newPercent}% ({_curLength.AsSize()} / {_maxLength.AsSize()})\n");
			}
			_curPercent = newPercent;
		}
	}
}

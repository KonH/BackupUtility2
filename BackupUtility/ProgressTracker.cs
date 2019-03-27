using System;

namespace BackupUtility {
	sealed class ProgressTracker {
		readonly long _maxLength;

		long _curLength  = 0;
		int  _curPercent = 0;

		public ProgressTracker(long maxLength) {
			_maxLength = maxLength;
		}

		public void UpdateProgress(long portion) {
			_curLength += portion;
			var newPercent = (int)Math.Round(((double)_curLength / _maxLength) * 100);
			if ( newPercent > _curPercent ) {
				Console.WriteLine($" {newPercent}% ({_curLength.AsSize()} / {_maxLength.AsSize()})");
			}
			_curPercent = newPercent;
		}
	}
}

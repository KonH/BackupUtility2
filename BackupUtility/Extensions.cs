using System;
using System.Linq;
using System.Collections.Generic;

namespace BackupUtility {
	static class Extensions {
		public static string AsSize(this long bytes) {
			var kb = (double)bytes / 1024;
			if ( kb < 1 ) {
				return Math.Round((double)bytes, 2) + " b";
			}
			var mb = kb / 1024;
			if ( mb < 1 ) {
				return Math.Round(kb, 2) + " kb";
			}
			var gb = mb / 1024;
			if ( gb < 1 ) {
				return Math.Round(mb, 2) + " mb";
			}
			return Math.Round(gb, 2) + " gb";
		}

		public static (List<T>, List<T>) Split<T>(this List<T> enumerable, Func<T, bool> selector) {
			var positive = enumerable.Where(selector).ToList();
			var negative = enumerable.Where(it => !selector(it)).ToList();
			return (positive, negative);
		}

		public static long SumByLength(this List<BackupTask> tasks) => tasks.Sum(t => t.File.Length);

		public static string AsDetails(this List<BackupTask> tasks) => $"{tasks.Count} files ({tasks.SumByLength().AsSize()})";
	}
}

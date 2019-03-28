using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace BackupUtility {
	sealed class Runner {
		readonly ILoggerFactory _loggerFactory;
		readonly ILogger        _logger;

		readonly string _sourcePath;
		readonly string _destinationPath;
		readonly int    _historyDepth;

		ProgressTracker _tracker;
		CacheWrapper   _cacheWrapper;

		Dictionary<string, CachedFileInfo> _cache => _cacheWrapper.Data;

		public Runner(ILoggerFactory loggerFactory, string sourcePath, string destinationPath, int historyDepth) {
			_loggerFactory   = loggerFactory;
			_logger          = loggerFactory.CreateLogger<Runner>();
			_sourcePath      = sourcePath;
			_destinationPath = destinationPath;
			_historyDepth    = historyDepth;
		}

		public void Process() {
			var cachePath = Path.Combine(_destinationPath, "__cache.json");
			using ( _cacheWrapper = new CacheWrapper(cachePath) ) {
				var allTasks = GetFiles(_sourcePath).Select(ToTask).ToList();

				var (wantedTasks, savedTasks) = allTasks.Split(t => t.IsRequired);
				_logger.LogInformation($"'{_sourcePath}' => '{_destinationPath}'");
				_logger.LogInformation($"Files to backup: {wantedTasks.AsDetails()}");
				_logger.LogInformation($"Saved by caching: {savedTasks.AsDetails()}\n");

				_tracker = new ProgressTracker(_loggerFactory, wantedTasks.SumByLength());

				var results = wantedTasks.Select(Execute).ToList();
				var (success, errors) = results.Split(t => t.IsSuccess);

				_logger.LogInformation($"Backup finished for {success.AsDetails()}");
				_logger.LogInformation($"Backup failed for {errors.AsDetails()}\n");
			}
		}

		IEnumerable<FileInfo> GetFiles(string path) => new DirectoryInfo(path).EnumerateFiles("", SearchOption.AllDirectories);

		BackupTask ToTask(FileInfo file) {
			var relativePath = Path.GetRelativePath(_sourcePath, file.FullName);
			return new BackupTask {
				File         = file,
				RelativePath = relativePath,
				IsRequired   = IsFileRequired(file, relativePath)
			};
		}

		bool IsFileRequired(FileInfo file, string relativePath) {
			if ( !_cache.TryGetValue(relativePath, out var cachedInfo) ) {
				return true;
			}
			if ( file.LastWriteTimeUtc > cachedInfo.LastWriteTimeUtc ) {
				return true;
			}
			if ( file.Length != cachedInfo.Length ) {
				return true;
			}
			return false;
		}

		BackupTask Execute(BackupTask task) {
			try {
				var destPath = Path.Combine(_destinationPath, task.RelativePath);
				EnsureDirectory(Path.GetDirectoryName(destPath));
				TrySaveToHistory(destPath);
				File.Copy(task.File.FullName, destPath);
				task.IsSuccess = true;
				Save(task);
			} catch ( Exception e ) {
				_logger.LogError($"\"{task.RelativePath}\": {e}\n");
			}
			_tracker.UpdateProgress(task.File.Length);
			return task;
		}

		// dir/file.x => dir/__file.x/file.x.time
		void TrySaveToHistory(string path) {
			if ( !File.Exists(path) ) {
				return;
			}
			var parentDirPath  = Path.GetDirectoryName(path);
			var fileName       = Path.GetFileName(path);
			var historyDirPath = Path.Combine(parentDirPath, "__" + fileName);
			EnsureDirectory(historyDirPath);

			var lastWriteTime = new FileInfo(path).LastWriteTimeUtc.ToFileTimeUtc();
			var destPath      = Path.Combine(historyDirPath, $"{fileName}.{lastWriteTime}");
			File.Move(path, destPath);
			_logger.LogInformation($"'{path}': previous version saved to history at '{destPath}'\n");

			CleanUpHistory(historyDirPath);
		}

		void CleanUpHistory(string path) {
			var files = new DirectoryInfo(path).GetFiles().OrderBy(f => f.LastWriteTimeUtc).ToList();
			var deleteCount = Math.Max(files.Count - _historyDepth, 0);
			if ( deleteCount == 0 ) {
				return;
			}
			var filesToDelete = files.Take(deleteCount).ToList();
			_logger.LogInformation($"'{path}': remove {deleteCount} of {files.Count} files from history.\n");
			if ( deleteCount == files.Count ) {
				Directory.Delete(path, true);
				return;
			}
			filesToDelete.ForEach(f => f.Delete());
		}

		void EnsureDirectory(string path) {
			if ( !Directory.Exists(path) ) {
				Directory.CreateDirectory(path);
			}
		}

		void Save(BackupTask task) {
			_cache[task.RelativePath] = new CachedFileInfo {
				LastWriteTimeUtc = task.File.LastWriteTimeUtc,
				Length           = task.File.Length
			};
		}
	}
}

using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Karambolo.Extensions.Logging.File;

namespace BackupUtility {
	public static class EntryPoint {
		public static void Run() {
			var config = Config.FromFile("config.json");
			var provider = CreateServiceProvider(config);

			provider.GetRequiredService<Utility>().Process();
		}

		static IServiceProvider CreateServiceProvider(Config config) {
			var services = new ServiceCollection();	
			services.AddSingleton(config);
			services.AddSingleton<Utility>();
			services.AddLogging(builder => {
				builder.AddConsole();
				builder.AddFile();
				builder.Services.Configure<FileLoggerOptions>(opts => {
					var dt = DateTime.Now;
					opts.FallbackFileName = $"{dt.ToString("yyyy.mm.dd_hh.mm.ss")}.log";
				});
			});
			return services.BuildServiceProvider();
		}
	}
}

using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting.Compact;

namespace MedStat.WebAdmin.Classes.Configuration
{
	public static class SerilogConfigurator
	{
		/// <summary>
		/// Returns Serilog logger with default configuration.
		/// </summary>
		/// <returns></returns>
		public static Logger CreateDefaultLogger()
		{
			var logger = new LoggerConfiguration()
				.MinimumLevel.Debug()
				.MinimumLevel.Override("Microsoft", LogEventLevel.Information)
				// prevent logging Info from all .NET Core events
				.MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
				.Enrich.FromLogContext()
				.WriteTo.Console()
				/*
				.WriteTo.File(// new CompactJsonFormatter(), - logging in JSON format
					"Logs\\log.txt", 
					rollingInterval: RollingInterval.Day, 
					rollOnFileSizeLimit: true, // create a file set like: log.txt, log_001.txt, log_002.txt
					retainedFileCountLimit: null,
					shared: true //  enable multi-process shared log files
					// Buffered writes are not available when file sharing is enabled.
					//,buffered: true // permit the underlying stream to buffer writes
					)
				*/
				.CreateLogger();

			return logger;
		}


		// Extensions

		/// <summary>
		/// Reads Serilog configuration from IConfiguration service. <br/>
		/// Override default Serilog Logger with following Serilog.Sinks:
		/// <para>
		/// - Serilog.Sinks.Console <br/>
		/// - Serilog.Sinks.File <br/>
		/// - Serilog.Sinks.MongoDB
		/// </para>
		/// </summary>
		/// <param name="host"></param>
		public static void SetupSerilog(this IHost host)
		{
			using (var scope = host.Services.CreateScope())
			{
				var services = scope.ServiceProvider;
				try
				{
					var configuration = services.GetRequiredService<IConfiguration>();

					SetPasswordForMongoDb(configuration);

					// Override default Serilog configuration
					Log.Logger = new LoggerConfiguration()
						.ReadFrom.Configuration(configuration)
						.CreateLogger();
				}
				catch (Exception ex)
				{
					var logger = services.GetRequiredService<ILogger<Program>>();
					logger.LogError(ex, "An error occurred at Serilog setup.");
				}
			}
		}


		private static void SetPasswordForMongoDb(IConfiguration configuration)
		{
			var serilogSettings = configuration.GetSection("Serilog").AsEnumerable();
			var databaseUrlSettings = serilogSettings
				.FirstOrDefault(s => s.Value != null && s.Value.StartsWith("mongodb://"));

			if (!string.IsNullOrEmpty(databaseUrlSettings.Value))
			{
				// Override user password for MongoDB
				configuration[databaseUrlSettings.Key] = databaseUrlSettings.Value.Replace("{userPassword}",
					configuration["Passwords:Serilog:MongoDB:UserPassword"]);
			}
		}
	}
}

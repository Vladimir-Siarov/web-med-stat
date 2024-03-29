using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using MedStat.WebAdmin.Classes.Configuration;
using Serilog;

namespace MedStat.WebAdmin
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			// Used for display any start errors such as read config section and etc.
			Log.Logger = SerilogConfigurator.CreateDefaultLogger();

			try
			{
				var host = CreateHostBuilder(args).Build();
				{
					host.SetupSerilog(); // custom method: Reconfigure Serilog. Adds new log targets (file and etc.)

					await host.SetupRolesAsync(); // custom method: Add user roles to DB if required
					await host.SetupSystemAdminAsync(); // custom method: setup System Admin record
				}
				
				Log.Information("Starting web host");
				host.Run();
			}
			catch (Exception ex)
			{
				Log.Fatal(ex, "Host terminated unexpectedly");
				throw;
			}
			finally
			{
				Log.CloseAndFlush();
			}
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.UseSerilog() // Add Serilog log provider
				.ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
	}
}

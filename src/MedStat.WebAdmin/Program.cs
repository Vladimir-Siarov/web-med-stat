using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using MedStat.WebAdmin.Classes.Configuration;

namespace MedStat.WebAdmin
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			var host = CreateHostBuilder(args).Build();
			{
				await host.SetupRolesAsync(); // custom method: Add user roles to DB if required
				await host.SetupSystemAdminAsync(); // custom method: setup System Admin record
			}
			host.Run();
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
	}
}

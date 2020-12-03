using System;
using System.Threading.Tasks;
using MedStat.Core.Identity;
using MedStat.Core.Repositories;
using MedStat.WebAdmin.Classes.Configuration.Sections;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MedStat.WebAdmin.Classes.Configuration
{
	public static class HostExtensions
	{
		/// <summary>
		/// Add user roles to DB if required.
		/// </summary>
		/// <param name="host"></param>
		/// <returns></returns>
		public static async Task SetupRolesAsync(this IHost host)
		{
			using (var scope = host.Services.CreateScope())
			{
				var services = scope.ServiceProvider;
				try
				{
					var rolesManager = services.GetRequiredService<RoleManager<IdentityRole<Int32>>>();
					var securityRepository = services.GetRequiredService<SecurityRepository>();

					await securityRepository.SetupRolesAsync(rolesManager);
				}
				catch (Exception ex)
				{
					var logger = services.GetRequiredService<ILogger<Program>>();
					logger.LogError(ex, "An error occurred at Roles setup.");
				}
			}
		}

		/// <summary>
		/// Create System Admin record (based on site configuration) in DB if required.<br/>
		/// Add specified User to the "SystemAdmin" role.
		/// </summary>
		/// <param name="host"></param>
		/// <returns></returns>
		public static async Task SetupSystemAdminAsync(this IHost host)
		{
			using (var scope = host.Services.CreateScope())
			{
				var services = scope.ServiceProvider;
				try
				{
					var configuration = services.GetRequiredService<IConfiguration>();
					var adminSettings = configuration.GetSection(SystemAdminSection.SectionName)
						.Get<SystemAdminSection>();

					if (!string.IsNullOrEmpty(adminSettings.Email))
					{
						var userManager = services.GetRequiredService<UserManager<SystemUser>>();
						var securityRepository = services.GetRequiredService<SecurityRepository>();

						await securityRepository.SetupSystemAdminAsync(userManager, 
							adminSettings.Email, adminSettings.Password);
					}
				}
				catch (Exception ex)
				{
					var logger = services.GetRequiredService<ILogger<Program>>();
					logger.LogError(ex, "An error occurred at System Admin setup.");
				}
			}
		}
	}
}

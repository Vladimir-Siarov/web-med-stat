﻿using MedStat.Core.DAL;
using MedStat.Core.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MedStat.WebAdmin.Classes.Configuration
{
	public static partial class ServicesExtensions
	{
		/// <summary>
		/// Registers "MedStat.Core" Repositories to the specified IServiceCollection.
		/// <para>
		/// NOTE: Method requires that IHttpContextAccessor singleton was registered in IServiceCollection.
		/// </para> 
		/// </summary>
		/// <param name="services">The <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceCollection" /> to add the service to.</param>
		/// <returns>A reference to this instance after the operation has completed.</returns>
		public static IServiceCollection AddCoreRepositories(this IServiceCollection services)
		{
			return
				services
					.AddScoped(sp => new SecurityRepository(
						sp.GetRequiredService<MedStatDbContext>(),
						sp.GetRequiredService<ILogger<SecurityRepository>>()
					))
					.AddScoped(sp => new CompanyRepository(
						sp.GetRequiredService<MedStatDbContext>(),
						sp.GetRequiredService<ILogger<CompanyRepository>>(),
						sp.GetRequiredService<IHttpContextAccessor>().HttpContext?.User?.Identity.Name
					));
		}
	}
}

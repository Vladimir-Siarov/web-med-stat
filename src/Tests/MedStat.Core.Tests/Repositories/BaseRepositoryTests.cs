﻿using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

using MedStat.Core.DAL;
using MedStat.Core.Identity;
using MedStat.Core.Interfaces;
using MedStat.Core.Repositories;

namespace MedStat.Core.Tests.Repositories
{
	public abstract class BaseRepositoryTests
	{
		public const string TestUserUid = "repository_tests";


		protected ITestOutputHelper OutputHelper { get; }


		protected BaseRepositoryTests(ITestOutputHelper outputHelper)
		{
			this.OutputHelper = outputHelper;
		}


		public ServiceProvider GetServiceProvider(MedStatDbContext dbContext,
			UserManager<SystemUser> customUserManager = null)
		{
			var services = new ServiceCollection();
			{
				// Add ILogger
				services.AddLogging(builder => builder.AddXunit(this.OutputHelper));

				// Add MedStatDbContext
				services.AddSingleton(dbContext);

				// Add Identity
				services
					.AddIdentity<SystemUser, IdentityRole<Int32>>() //(options => options.SignIn.RequireConfirmedAccount = true)
					.AddRoles<IdentityRole<Int32>>()
					.AddEntityFrameworkStores<MedStatDbContext>();

				// Register Repositories
				services
					.AddScoped<IIdentityRepository>(sp => new IdentityRepository(
						customUserManager ?? sp.GetRequiredService<UserManager<SystemUser>>(),
						sp.GetRequiredService<MedStatDbContext>(),
						sp.GetRequiredService<ILogger<IdentityRepository>>(),
						TestUserUid))
					
					.AddScoped<ISecurityRepository>(sp => new SecurityRepository(
						sp.GetRequiredService<IIdentityRepository>(),
						sp.GetRequiredService<MedStatDbContext>(),
						sp.GetRequiredService<ILogger<SecurityRepository>>()
					))

					.AddScoped<ICompanyRepository>(sp => new CompanyRepository(
						sp.GetRequiredService<IIdentityRepository>(),
						sp.GetRequiredService<MedStatDbContext>(),
						sp.GetRequiredService<ILogger<CompanyRepository>>(),
						TestUserUid
					));
			}

			var serviceProvider = services.BuildServiceProvider();

			return serviceProvider;
		}

		public async Task InitRolesAsync(MedStatDbContext dbContext)
		{
			var sp = this.GetServiceProvider(dbContext);
			var securityRepository = sp.GetRequiredService<ISecurityRepository>();
			var roleManager = sp.GetRequiredService<RoleManager<IdentityRole<Int32>>>();

			await securityRepository.SetupRolesAsync(roleManager);
		}
	}
}

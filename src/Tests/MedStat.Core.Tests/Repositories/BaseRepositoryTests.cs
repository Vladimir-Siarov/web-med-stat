using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.DataProtection;

using Xunit;
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

		protected DatabaseFixture Fixture { get; }


		protected BaseRepositoryTests(ITestOutputHelper outputHelper, DatabaseFixture fixture)
		{
			this.OutputHelper = outputHelper;
			this.Fixture = fixture;
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
					.AddEntityFrameworkStores<MedStatDbContext>()
					.AddDefaultTokenProviders();

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
					))

					// Register "CompanyRepository" as "ICompanyUserRepository", because we wont to test that interface separately.
					.AddScoped<ICompanyUserRepository>(sp => new CompanyRepository(
						sp.GetRequiredService<IIdentityRepository>(),
						sp.GetRequiredService<MedStatDbContext>(),
						sp.GetRequiredService<ILogger<CompanyRepository>>(),
						TestUserUid
					));

				// Define custom encryption keys store, for prevent standard (DPAPI) encryption info output in each test:
				//   "User profile is available. Using 'C:\Users\XXX\AppData\Local\ASP.NET\DataProtection-Keys' as key repository and Windows DPAPI to encrypt keys at rest."
				var currDirPath = System.IO.Directory.GetCurrentDirectory();
				services.AddDataProtection()
					.PersistKeysToFileSystem(new System.IO.DirectoryInfo($"{currDirPath}\\PersistKeys"));
			}

			var serviceProvider = services.BuildServiceProvider();

			return serviceProvider;
		}


		public void InitRolesIfRequired()
		{
			if (!this.Fixture.IsRolesInitialized)
			{
				using (var dbContext = new MedStatDbContext(this.Fixture.ContextOptions))
				{
					Task.Run(() => this.InitRolesAsync(dbContext)).Wait();
				}
			}
		}

		private async Task InitRolesAsync(MedStatDbContext dbContext)
		{
			var sp = this.GetServiceProvider(dbContext);
			var securityRepository = sp.GetRequiredService<ISecurityRepository>();
			var roleManager = sp.GetRequiredService<RoleManager<IdentityRole<Int32>>>();

			this.OutputHelper.WriteLine("--- start init Roles for tests ---");
			{
				await securityRepository.SetupRolesAsync(roleManager);

				this.Fixture.IsRolesInitialized = true;
			}
			this.OutputHelper.WriteLine("--- end of Roles init for tests ---\n");
		}


		public async Task<int> AddSystemUserToDbAsync(SystemUser userData,
			IEnumerable<string> userRoles = null, string password = null)
		{
			using (var dbContext = new MedStatDbContext(this.Fixture.ContextOptions))
			{
				var sp = this.GetServiceProvider(dbContext);
				var userManager = sp.GetRequiredService<UserManager<SystemUser>>();

				var createUserResult = password != null
					? await userManager.CreateAsync(userData, password)
					: await userManager.CreateAsync(userData);
				if (!createUserResult.Succeeded)
					throw new Exception("Test arrange is failed. Cannot add SystemUser entity to DB.");

				if (userRoles != null)
				{
					var addToRolesResult = await userManager.AddToRolesAsync(userData, userRoles);
					if (!addToRolesResult.Succeeded)
						throw new Exception("Test arrange is failed. Cannot add SystemUser to roles.");
				}

				return
					userData.Id;
			}
		}

		public void CheckUserRoles(int userId, IEnumerable<string> expectedUserRoles,
			bool checkIsNotInExpectedRoles = false)
		{
			using (var dbContext = new MedStatDbContext(this.Fixture.ContextOptions))
			{
				var dbUser = dbContext.SystemUsers.FirstOrDefault(su => su.Id == userId);

				dbUser.Should().NotBeNull();

				var sp = this.GetServiceProvider(dbContext);
				var userManager = sp.GetRequiredService<UserManager<SystemUser>>();

				var dbUserRoles = userManager.GetRolesAsync(dbUser).Result;

				if (checkIsNotInExpectedRoles) // check that user NOT IN expected roles
				{
					foreach (string expectedRole in expectedUserRoles)
					{
						Assert.DoesNotContain(expectedRole, dbUserRoles);
					}
				}
				else // check that user IN expected roles
				{
					Assert.True(dbUserRoles.Count == expectedUserRoles.Count());
					foreach (string expectedRole in expectedUserRoles)
					{
						Assert.Contains(expectedRole, dbUserRoles);
					}
				}
			}
		}
	}
}

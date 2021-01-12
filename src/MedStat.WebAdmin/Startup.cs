using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using MedStat.Core.DAL;
using MedStat.Core.Identity;
using MedStat.WebAdmin.Classes;
using MedStat.WebAdmin.Classes.Configuration;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.WebEncoders;

namespace MedStat.WebAdmin
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			//services.AddDatabaseDeveloperPageExceptionFilter();

			services.AddScoped(sp => 
			{
				string connectionString = this.Configuration.GetConnectionString("MedStatConnectionString");
				return 
					new MedStatDbContext(connectionString);
			});

			services
				.AddDefaultIdentity<SystemUser>()//(options => options.SignIn.RequireConfirmedAccount = true)
				.AddRoles<IdentityRole<Int32>>()
				.AddEntityFrameworkStores<MedStatDbContext>()
				.AddClaimsPrincipalFactory<SystemUserClaimsPrincipalFactory>(); // custom factory: adds custom claims

			services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
			services.AddCoreRepositories(); // custom method: add repositories from "Core" project

			services.AddLocalization(options => options.ResourcesPath = "Resources");

			services
				.AddRazorPages(options => { options.Conventions.AuthorizeFolder("/Companies"); })
				//.AddRazorRuntimeCompilation();
				.AddCustomDataAnnotationsLocalization() // custom method: add DA localization for "Core" BEs and project's ViewModels
				.AddViewLocalization();

			services.ConfigureRequestLocalization(); // custom method: setup Web App localization
			services.Configure<WebEncoderOptions>(options =>
			{
				options.TextEncoderSettings = new TextEncoderSettings(UnicodeRanges.BasicLatin,
					// Prevent encoding russian words (from Resources and DB) to Unicode codes (like as "&#x41D;") at HTML!
					UnicodeRanges.Cyrillic);
			});

			services.AddAuthorization(options =>
			{
				options.AddPolicy(AccessPolicies.CompanyUserManageRights, policy =>
					policy.RequireRole(AccessPolicies.Roles[AccessPolicies.CompanyUserManageRights]));
			});
		

			// Setup data protection for Web farm: 
			// (and prevent "The antiforgery token could not be decrypted" error)
			// https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/iis/?view=aspnetcore-2.1#data-protection-2
			// https://docs.microsoft.com/en-us/aspnet/core/security/data-protection/configuration/overview?view=aspnetcore-2.1#persistkeystofilesystem
			// NOTE: Switch off "Load User Profile" for app pool.
			var currDirPath =  System.IO.Directory.GetCurrentDirectory();
			services.AddDataProtection()
				.PersistKeysToFileSystem(new System.IO.DirectoryInfo($"{currDirPath}\\..\\PersistKeys"));
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			// env.EnvironmentName = "Production"; TODO: For test purpose only (Error pages)
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				//app.UseMigrationsEndPoint();
			}
			else
			{
				app.UseExceptionHandler("/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			// handle 400, 404 and other HTTP status codes
			app.UseStatusCodePagesWithReExecute("/error", "?httpCode={0}");

			app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseRouting();

			app.UseAuthentication();
			app.UseAuthorization();

			app.UseRequestLocalization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapRazorPages();
			});
		}
	}
}

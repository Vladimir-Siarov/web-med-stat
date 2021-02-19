using System;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.WebEncoders;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using MedStat.Core.DAL;
using MedStat.Core.Identity;
using MedStat.WebAdmin.Classes;
using MedStat.WebAdmin.Classes.Configuration;
using Newtonsoft.Json.Converters;

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
				string connectionString = this.Configuration
					.GetConnectionString("MedStat.WebAdmin.ConnectionString")
					// read password from secrets or env.
					// NOTE: Be careful, under IIS profile the "secrets.json" file is looked in other place
					.Replace("{userPassword}", this.Configuration["Passwords:MedStat.WebAdmin.ConnectionString:UserPassword"]);

				return 
					new MedStatDbContext(connectionString);
			});

			services
				.AddIdentity<SystemUser, IdentityRole<Int32>>()//(options => options.SignIn.RequireConfirmedAccount = true)
				.AddDefaultTokenProviders()
				.AddRoles<IdentityRole<Int32>>()
				.AddEntityFrameworkStores<MedStatDbContext>()
				.AddClaimsPrincipalFactory<SystemUserClaimsPrincipalFactory>(); // custom factory: adds custom claims

			services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
			services.AddCoreRepositories(); // custom method: add repositories from "Core" project

			services.AddLocalization(options => options.ResourcesPath = "Resources");

			services
				.AddRazorPages(options => { options.Conventions.AuthorizeFolder("/Companies"); })
				//.AddRazorRuntimeCompilation(); - disable runtime compilation on PROD (on IIS)
				.AddCustomDataAnnotationsLocalization() // custom method: add DA localization for "Core" BEs and project's ViewModels
				.AddViewLocalization()
				.AddNewtonsoftJson(option =>
				{
					// Json serializer settings: Enum as string and not ignore null values
					option.SerializerSettings.Converters.Add(new StringEnumConverter());
					option.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Include;
				});

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
				.PersistKeysToFileSystem(new System.IO.DirectoryInfo($"{currDirPath}\\PersistKeys"));
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

			//app.UseHttpsRedirection();
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

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MedStat.Core.DAL;
using MedStat.Core.Identity;
using Microsoft.AspNetCore.DataProtection;

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

			//services.AddDbContext<MedStatDbContext>();
			services.AddScoped(sp => 
			{
				string connectionString = this.Configuration.GetConnectionString("MedStatConnectionString");
				return 
					new MedStatDbContext(connectionString);
			});

			services
				.AddDefaultIdentity<SystemUser>()//(options => options.SignIn.RequireConfirmedAccount = true)
				.AddEntityFrameworkStores<MedStatDbContext>();

			services
				.AddRazorPages(options =>
				{
					options.Conventions.AuthorizeFolder("/Companies");
				});
			//.AddRazorRuntimeCompilation();
			
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

			app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseRouting();

			app.UseAuthentication();
			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapRazorPages();
			});
		}
	}
}

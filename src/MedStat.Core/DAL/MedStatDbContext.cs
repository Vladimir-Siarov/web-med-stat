using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

using MedStat.Core.Identity;

namespace MedStat.Core.DAL
{
	public class MedStatDbContext : IdentityDbContext<SystemUser, IdentityRole<Int32>, Int32>
	{
		private string _defaultConnectionString;
		
		
		//public DbSet<Company> Companies { get; set; }


		protected string ConnectionString { get; }
				
		protected string DefaultConnectionString 
		{
			get 
			{
				if (string.IsNullOrEmpty(_defaultConnectionString)) 
				{
					_defaultConnectionString = GetDefaultConnectionString();
				}

				return _defaultConnectionString;
			}
		}


		public MedStatDbContext(string connectionString) 
		{
			this.ConnectionString = connectionString;
		}


		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			base.OnConfiguring(optionsBuilder);

			if (!optionsBuilder.IsConfigured)
			{
				//optionsBuilder.UseSqlServer("name=MedStatConnectionString"); - doesn't work

				if (!string.IsNullOrEmpty(this.ConnectionString))
				{
					optionsBuilder.UseSqlServer(this.ConnectionString);
				}
				else 
				{
					optionsBuilder.UseSqlServer(this.DefaultConnectionString);
				}
			}
		}

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);

			// Override default AspNet Identity table names
			builder.Entity<SystemUser>(b => 
			{ 
				b.ToTable(name: "SystemUser"); 
			});			
		}


		/// <summary>
		/// Use for Create DB migration via PM console.
		/// </summary>
		/// <returns></returns>
		private string GetDefaultConnectionString() 
		{
			// add IConfigurationRoot  to get connection string 
			IConfigurationRoot configuration = new ConfigurationBuilder()
					.SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
					.AddJsonFile("appsettings.json")
					.Build();

			return
				configuration.GetConnectionString("MedStatConnectionString");
		}
	}
}

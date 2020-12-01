using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using MedStat.Core.BE.Company;
using MedStat.Core.Identity;

namespace MedStat.Core.DAL
{
	public class MedStatDbContext : IdentityDbContext<SystemUser, IdentityRole<Int32>, Int32>
	{
		private string _defaultConnectionString;
		
		
		public DbSet<Company> Companies { get; set; }


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
			Console.WriteLine("MedStatDbContext ctr");
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


			// Company
			{
				// CompanyMainRequisites
				builder.Entity<Company>().OwnsOne(
					c => c.MainRequisites
					//, Moved to Data Annotation attributes
					//cmr =>
					//{
					//	cmr.Property(c => c.Name).HasMaxLength(50)
					//		// bag: doesn't work in EF Core 3.0.x https://github.com/dotnet/efcore/issues/16943
					//		// fix: manually update the generated migration
					//		.IsRequired(); // bag: doesn't work in EF Core 3.0
					//	cmr.Property(c => c.FullName).HasMaxLength(150);

					//	cmr.Property(c => c.LegalAddress).HasMaxLength(300);
					//	cmr.Property(c => c.PostalAddress).HasMaxLength(300);

					//	cmr.Property(c => c.OGRN).HasMaxLength(50);
					//	cmr.Property(c => c.OKPO).HasMaxLength(50);
					//	cmr.Property(c => c.OKATO).HasMaxLength(50);
					//	cmr.Property(c => c.INN).HasMaxLength(50);
					//	cmr.Property(c => c.KPP).HasMaxLength(50);
					//}
					);

				// CompanyBankRequisites
				builder.Entity<Company>().OwnsOne(
					c => c.BankRequisites
					//, Moved to Data Annotation attributes
					//br =>
					//{
					//	br.Property(c => c.AccountNumber).HasMaxLength(50);
					//	br.Property(c => c.BIC).HasMaxLength(50);
					//	br.Property(c => c.CorrespondentAccount).HasMaxLength(50);
					//	br.Property(c => c.Bank).HasMaxLength(300);
					//}
					);
			}
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

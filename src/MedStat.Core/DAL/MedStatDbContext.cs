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


		public DbSet<SystemUser> SystemUsers { get; set; }

		public DbSet<Company> Companies { get; set; }

		public DbSet<CompanyRequisites> CompanyRequisites { get; set; }

		public DbSet<CompanyUser> CompanyUsers { get; set; }


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
			Console.WriteLine("MedStatDbContext ctr call"); // TODO: For dev only
		}

		// For tests setup
		public MedStatDbContext(DbContextOptions<MedStatDbContext> options)
			: base(options)
		{
		}


		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			base.OnConfiguring(optionsBuilder);

			if (!optionsBuilder.IsConfigured)
			{
				//optionsBuilder.UseSqlServer("name=MedStat.WebAdmin.ConnectionString"); - doesn't work

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

			// SystemUsers
			builder.Entity<SystemUser>(b => 
			{ 
				b.ToTable(name: "SystemUsers"); // override default AspNet Identity table names
				b.Property(su => su.IsPasswordChangeRequired).HasColumnName("PasswordChangeRequired");
				b.Property(su => su.NormalizedPhoneNumber).IsRequired();
			});


			// CompanyRequisites
			{
				// CompanyMainRequisites
				builder.Entity<CompanyRequisites>().OwnsOne(c => c.MainRequisites,
					cmr =>
					{
						// Moved to Data Annotation attributes, but doesn't work also ...
						cmr.Property(mr => mr.Name).IsRequired();
					});

				// CompanyBankRequisites
				builder.Entity<CompanyRequisites>().OwnsOne(c => c.BankRequisites);
			}

			// CompanyUser
			{
				builder.Entity<CompanyUser>()
					.HasOne(cu => cu.Login)
					.WithOne().HasForeignKey<CompanyUser>(c => c.SystemUserId)
					.IsRequired()
					.OnDelete(DeleteBehavior.ClientSetNull);

				builder.Entity<CompanyUser>().ToTable("CompanyUsers");
			}

			// Company
			{
				builder.Entity<Company>()
					.HasOne(c => c.Requisites)
					.WithOne().HasForeignKey<CompanyRequisites>(r => r.CompanyId)
					.IsRequired()
					// If CompanyRequisites wasn't deleted before,
					// then throws an error at in memory db and leads to constraint violation fail in physical db
					.OnDelete(DeleteBehavior.ClientSetNull);

				builder.Entity<Company>()
					.HasMany(c => c.Users)
					.WithOne().HasForeignKey(cu => cu.CompanyId)
					.IsRequired()
					// If Company wasn't deleted before,
					// then throws an error at in memory db and leads to constraint violation fail in physical db
					.OnDelete(deleteBehavior: DeleteBehavior.ClientSetNull);
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
					.AddUserSecrets<MedStatDbContext>()
					.Build();

			string connectionString = configuration
				.GetConnectionString("MedStat.WebAdmin.ConnectionString")
				// read password from secrets or env.
				.Replace("{userPassword}", configuration["Passwords:MedStat.WebAdmin.ConnectionString:UserPassword"]);

			return 
				connectionString;
		}
	}
}

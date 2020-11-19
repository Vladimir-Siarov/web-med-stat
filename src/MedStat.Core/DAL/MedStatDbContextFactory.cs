using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore.Design;

namespace MedStat.Core.DAL
{
	public class MedStatDbContextFactory : IDesignTimeDbContextFactory<MedStatDbContext>
	{
		public MedStatDbContext CreateDbContext(string[] args)
		{
			//			var configuration = new ConfigurationBuilder()
			//				.SetBasePath(Directory.GetCurrentDirectory())
			//				.AddJsonFile("appsettings.json")
			//				.Build();
			//
			//			var optionsBuilder = new DbContextOptionsBuilder();
			//
			//			var connectionString = configuration
			//				.GetConnectionString("DefaultConnection");
			//
			//			optionsBuilder.UseSqlServer(connectionString);
			//
			//			return new MedStatDbContext(optionsBuilder.Options);
			return new MedStatDbContext(null);
		}
  }
}

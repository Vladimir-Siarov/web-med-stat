using System;
using System.Data.Common;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

using MedStat.Core.DAL;

namespace MedStat.Core.Tests
{
	public class DatabaseFixture : IDisposable
	{
		private readonly DbConnection _connection;


		public DbContextOptions<MedStatDbContext> ContextOptions { get; }

		public bool IsRolesInitialized { get; set; }


		public DatabaseFixture()
		{
			_connection = new SqliteConnection("Filename=:memory:");
			_connection.Open();

			this.ContextOptions = new DbContextOptionsBuilder<MedStatDbContext>()
				.UseSqlite(_connection)
				.Options;

			this.Seed();
		}

		public void Dispose()
		{
			_connection.Dispose();
		}


		private void Seed()
		{
			using (var context = new MedStatDbContext(this.ContextOptions))
			{
				context.Database.EnsureDeleted();
				context.Database.EnsureCreated();

				// ... initialize data in the test database ...
			}
		}
  }
}

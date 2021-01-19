using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using MedStat.Core.BE.Company;
using MedStat.Core.DAL;
using MedStat.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace MedStat.Core.Tests.Repositories
{
	public class CompanyRepositoryTests : BaseRepositoryTests, IClassFixture<DatabaseFixture>
	{
		private DatabaseFixture _fixture;


		public CompanyRepositoryTests(DatabaseFixture fixture, ITestOutputHelper outputHelper)
		: base(outputHelper)
		{
			this._fixture = fixture;
		}


		#region Get

		[Fact]
		public void GetCompanyMainDataAsync()
		{
			// Arrange:

			var expectedCompany = new Company
			{
				Name = "Test company 1",
				Description = "Test company description",
				CreatedUtc = DateTime.UtcNow.AddDays(-1),
				UpdatedUtc = DateTime.UtcNow
			};

			using (var context = new MedStatDbContext(_fixture.ContextOptions))
			{
				context.Companies.Add(expectedCompany);
				context.SaveChanges();
			}


			// Act:

			Company company;
			using (var dbContext = new MedStatDbContext(_fixture.ContextOptions))
			{
				var sp = this.GetServiceProvider(dbContext);
				var cmpRepository = sp.GetRequiredService<ICompanyRepository>();

				company = cmpRepository.GetCompanyMainDataAsync(expectedCompany.Id).Result;
			}


			// Assert:

			company.Should().NotBeNull();
			company.Id.Should().Be(expectedCompany.Id);
			company.Description.Should().BeEquivalentTo(expectedCompany.Description);
			company.CreatedUtc.Should().Be(expectedCompany.CreatedUtc);
			company.UpdatedUtc.Should().Be(expectedCompany.UpdatedUtc);
		}

		#endregion
	}
}

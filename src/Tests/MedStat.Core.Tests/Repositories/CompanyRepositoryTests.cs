using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using MedStat.Core.BE.Company;
using MedStat.Core.DAL;
using MedStat.Core.Identity;
using MedStat.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
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

		[Fact]
		public void GetCompanyWithRequisitesAsync()
		{
			#region Arrange:

			var guid = Guid.NewGuid();
			var expectedCompany = new Company
			{
				Name = guid.ToString(),
				Description = $"{guid} description",
				CreatedUtc = DateTime.UtcNow.AddDays(-1),
				UpdatedUtc = DateTime.UtcNow.AddDays(-1),

				Requisites = GetCompanyRequisitesFullData()
			};


			using (var context = new MedStatDbContext(_fixture.ContextOptions))
			{
				context.Companies.Add(expectedCompany);
				context.SaveChanges();
			}

			#endregion

			// Act:

			Company company;
			using (var dbContext = new MedStatDbContext(_fixture.ContextOptions))
			{
				var sp = this.GetServiceProvider(dbContext);
				var cmpRepository = sp.GetRequiredService<ICompanyRepository>();

				company = cmpRepository.GetCompanyWithRequisitesAsync(expectedCompany.Id).Result;
			}


			// Assert:

			company.Should().NotBeNull();
			company.Id.Should().Be(expectedCompany.Id);

			company.Requisites.Should().NotBeNull();
			company.Requisites.UpdatedUtc.Should().Be(expectedCompany.Requisites.UpdatedUtc);

			company.Requisites.MainRequisites.Should().NotBeNull();
			CompareMainRequisites(company.Requisites.MainRequisites, expectedCompany.Requisites.MainRequisites);

			company.Requisites.BankRequisites.Should().NotBeNull();
			CompareBankRequisites(company.Requisites.BankRequisites, expectedCompany.Requisites.BankRequisites);
		}

		#endregion

		#region Create

		[Fact]
		public void CreateCompanyAsync()
		{
			// Arrange:

			var guid = Guid.NewGuid();
			var expectedData = new Company
			{
				Name = guid.ToString(),
				Description = $"{guid} description"
			};


			// Act:

			int companyId;
			using (var dbContext = new MedStatDbContext(_fixture.ContextOptions))
			{
				var sp = this.GetServiceProvider(dbContext);
				var cmpRepository = sp.GetRequiredService<ICompanyRepository>();

				companyId = cmpRepository.CreateCompanyAsync(expectedData.Name, expectedData.Description).Result;
			}


			// Assert:

			using (var dbContext = new MedStatDbContext(_fixture.ContextOptions))
			{
				var company = dbContext.Companies
					.Include(c => c.Requisites)
					.FirstOrDefault(c => c.Id == companyId);

				company.Should().NotBeNull();

				company.Name.Should().BeEquivalentTo(expectedData.Name);
				company.Description.Should().BeEquivalentTo(expectedData.Description);

				company.CreatedUtc.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
				company.UpdatedUtc.Should().Be(company.CreatedUtc);

				// Test custom business logic
				company.Requisites.Should().NotBeNull();
				company.Requisites.MainRequisites.Name.Should().BeEquivalentTo(expectedData.Name);
			}
		}

		#endregion

		#region Update / Delete

		[Fact]
		public async void UpdateCompanyMainDataAsync()
		{
			#region Arrange:

			int companyId;
			using (var dbContext = new MedStatDbContext(_fixture.ContextOptions))
			{
				var guid = Guid.NewGuid();
				var companyData = new Company
				{
					Name = guid.ToString(),
					Description = $"{guid} description",
					CreatedUtc = DateTime.UtcNow.AddDays(-1),
					UpdatedUtc = DateTime.UtcNow.AddDays(-1)
				};

				dbContext.Companies.Add(companyData);
				dbContext.SaveChanges();

				companyId = companyData.Id;
			}

			var date = DateTime.UtcNow;
			var expectedData = new Company
			{
				Name = $"updated name: {date.ToShortDateString()} {date.ToShortTimeString()}",
				Description = $"updated description: {date.ToShortDateString()} {date.ToShortTimeString()}"
			};

			#endregion

			// Act:

			using (var dbContext = new MedStatDbContext(_fixture.ContextOptions))
			{
				var sp = this.GetServiceProvider(dbContext);
				var cmpRepository = sp.GetRequiredService<ICompanyRepository>();

				await cmpRepository.UpdateCompanyMainDataAsync(companyId, expectedData.Name, expectedData.Description);
			}


			// Assert:

			using (var dbContext = new MedStatDbContext(_fixture.ContextOptions))
			{
				var company = dbContext.Companies.FirstOrDefault(c => c.Id == companyId);

				company.Should().NotBeNull();

				company.Name.Should().BeEquivalentTo(expectedData.Name);
				company.Description.Should().BeEquivalentTo(expectedData.Description);

				company.UpdatedUtc.Should().BeAfter(company.CreatedUtc);
				company.UpdatedUtc.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
			}
		}

		[Theory]
		[MemberData(nameof(UpdateCompanyRequisitesAsync_Data))]
		public async void UpdateCompanyRequisitesAsync(Company existedCompanyData)
		{
			#region Arrange:

			int companyId;
			using (var dbContext = new MedStatDbContext(_fixture.ContextOptions))
			{
				dbContext.Companies.Add(existedCompanyData);
				dbContext.SaveChanges();

				companyId = existedCompanyData.Id;
			}

			var updatedPostfix = $" updated: {DateTime.UtcNow.ToShortDateString()} {DateTime.UtcNow.ToLongTimeString()}";
			var expectedData = new CompanyRequisites
			{
				MainRequisites = new CompanyMainRequisites
				{
					Name = "test_Name" + updatedPostfix,
					FullName = "test_FullName" + updatedPostfix,

					PostalAddress = "test_PostalAddress" + updatedPostfix,
					LegalAddress = "test_LegalAddress" + updatedPostfix,

					OKATO = "test_OKATO" + updatedPostfix,
					INN = "test_INN" + updatedPostfix,
					OGRN = "test_OGRN" + updatedPostfix,
					OKPO = "test_OKPO" + updatedPostfix,
					KPP = "test_KPP" + updatedPostfix
				},

				BankRequisites = new CompanyBankRequisites
				{
					AccountNumber = "test_AccountNumber" + updatedPostfix,
					CorrespondentAccount = "test_CorrespondentAccount" + updatedPostfix,
					BIC = "test_BIC" + updatedPostfix,
					Bank = "test_Bank" + updatedPostfix
				}
			};

			bool isValidCompanyData = existedCompanyData.Requisites != null;

			#endregion

			// Act:

			using (var dbContext = new MedStatDbContext(_fixture.ContextOptions))
			{
				var sp = this.GetServiceProvider(dbContext);
				var cmpRepository = sp.GetRequiredService<ICompanyRepository>();

				if (isValidCompanyData)
				{
					await cmpRepository.UpdateCompanyRequisitesAsync(companyId,
						expectedData.MainRequisites, expectedData.BankRequisites);
				}
				else
				{
					// act + assert
					var ex = await Assert.ThrowsAsync<OperationCanceledException>(async () =>
						await cmpRepository.UpdateCompanyRequisitesAsync(companyId,
							expectedData.MainRequisites, expectedData.BankRequisites));

					return;
				}
			}


			// Assert:

			using (var dbContext = new MedStatDbContext(_fixture.ContextOptions))
			{
				var company = dbContext.Companies
					.Include(c => c.Requisites)
					.FirstOrDefault(c => c.Id == companyId);

				company.Should().NotBeNull();

				company.Requisites.Should().NotBeNull();
				company.Requisites.UpdatedUtc.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));

				company.Requisites.MainRequisites.Should().NotBeNull();
				CompareMainRequisites(company.Requisites.MainRequisites, expectedData.MainRequisites);

				company.Requisites.BankRequisites.Should().NotBeNull();
				CompareBankRequisites(company.Requisites.BankRequisites, expectedData.BankRequisites);
			}
		}

		[Theory]
		[MemberData(nameof(DeleteCompanyAsync_Data))]
		public async void DeleteCompanyAsync(Company existedCompanyData)
		{
			// Arrange:

			int companyId;
			using (var dbContext = new MedStatDbContext(_fixture.ContextOptions))
			{
				dbContext.Companies.Add(existedCompanyData);
				dbContext.SaveChanges();

				companyId = existedCompanyData.Id;
			}

			bool companyCanBeDeleted = (existedCompanyData.Users == null || existedCompanyData.Users.Count == 0);


			// Act:

			using (var dbContext = new MedStatDbContext(_fixture.ContextOptions))
			{
				var sp = this.GetServiceProvider(dbContext);
				var cmpRepository = sp.GetRequiredService<ICompanyRepository>();

				if (companyCanBeDeleted)
				{
					await cmpRepository.DeleteCompanyAsync(companyId);
				}
				else
				{
					// act + assert
					var ex = await Assert.ThrowsAsync<OperationCanceledException>(async () =>
						await cmpRepository.DeleteCompanyAsync(companyId));

					return;
				}
			}


			// Assert:

			using (var dbContext = new MedStatDbContext(_fixture.ContextOptions))
			{
				bool isCompanyExist = dbContext.Companies.Any(c => c.Id == companyId);
				isCompanyExist.Should().BeFalse();

				bool isCmpRequisitesExist = dbContext.CompanyRequisites.Any(cr => cr.CompanyId == companyId);
				isCmpRequisitesExist.Should().BeFalse();

				bool isCmpUserExist = dbContext.CompanyUsers.Any(cu => cu.CompanyId == companyId);
				isCmpUserExist.Should().BeFalse();
			}
		}

		#endregion

		// TODO: Tests for FindCompaniesAsync


		// Data for Test methods:

		public static IEnumerable<object[]> UpdateCompanyRequisitesAsync_Data
		{
			get
			{
				// Company without Requisites (error in business logic)
				var companyWithoutRequisites = new Company
				{
					Name = $"Company without Requisites {Guid.NewGuid()}",
					CreatedUtc = DateTime.UtcNow.AddDays(-1)
				};

				var companyWithoutBankRequisites = new Company
				{
					Name = $"Company without BankRequisites {Guid.NewGuid()}",
					CreatedUtc = DateTime.UtcNow.AddDays(-1),

					Requisites = new CompanyRequisites
					{
						MainRequisites = new CompanyMainRequisites
						{
							Name = "UpdateCompanyRequisitesAsync_Data Name 2"
						},

						BankRequisites = null
					}
				};

				var companyWithFullRequisites = new Company
				{
					Name = $"Company with full Requisites {Guid.NewGuid()}",
					CreatedUtc = DateTime.UtcNow.AddDays(-1),

					Requisites = GetCompanyRequisitesFullData()
				};

				return
					new List<object[]>
					{
						new object[] { companyWithoutRequisites },
						new object[] { companyWithoutBankRequisites },
						new object[] { companyWithFullRequisites }
					};
			}
		}
		
		public static IEnumerable<object[]> DeleteCompanyAsync_Data
		{
			get
			{
				var emptyCompany = new Company
				{
					Name = $"Empty Company {Guid.NewGuid()}",
					CreatedUtc = DateTime.UtcNow.AddDays(-1)
				};

				var companyWithRequisites = new Company
				{
					Name = $"Company with Requisites {Guid.NewGuid()}",
					CreatedUtc = DateTime.UtcNow.AddDays(-1),

					Requisites = GetCompanyRequisitesFullData()
				};

				var companyWithCmpUsers = new Company
				{
					Name = $"Company with CompanyUsers {Guid.NewGuid()}",
					CreatedUtc = DateTime.UtcNow.AddDays(-1),

					Users = GetCompanyUsersData()
				};

				return 
					new List<object[]>
					{
						new object[] { emptyCompany },
						new object[] { companyWithRequisites },
						new object[] { companyWithCmpUsers }
					};
			}
		}


		// Helpers:

		public static CompanyRequisites GetCompanyRequisitesFullData()
		{
			return
				new CompanyRequisites
				{
					UpdatedUtc = DateTime.UtcNow.AddDays(-1),

					MainRequisites = new CompanyMainRequisites
					{
						Name = "test_Name",
						FullName = "test_FullName",

						PostalAddress = "test_PostalAddress",
						LegalAddress = "test_LegalAddress",

						OKATO = "test_OKATO",
						INN = "test_INN",
						OGRN = "test_OGRN",
						OKPO = "test_OKPO",
						KPP = "test_KPP"
					},

					BankRequisites = new CompanyBankRequisites
					{
						AccountNumber = "test_AccountNumber",
						CorrespondentAccount = "test_CorrespondentAccount",
						BIC = "test_BIC",
						Bank = "test_Bank"
					}
				};
		}

		// M.b. move this method to the "IdentityRepositoryTests" class
		public static List<CompanyUser> GetCompanyUsersData()
		{
			var phoneNumber1 = "+7 111 11-11-11";
			var phoneNumber2 = "+7 222 22-22-22";

			return
				new List<CompanyUser>
				{
					new CompanyUser
					{
						Login = new SystemUser
						{
							UserName = phoneNumber1,
							PhoneNumber = phoneNumber1,
							NormalizedPhoneNumber = phoneNumber1,
							FirstName = "First 1",
							Surname = "Surname 1"
						}
					},
					new CompanyUser
					{
						Login = new SystemUser
						{
							UserName = phoneNumber2,
							PhoneNumber = phoneNumber2,
							NormalizedPhoneNumber = phoneNumber2,
							FirstName = "First 1",
							Surname = "Surname 1"
						}
					}
				};
		}


		private static void CompareMainRequisites(CompanyMainRequisites requisites, 
			CompanyMainRequisites expectedRequisites)
		{
			requisites.Name.Should().BeEquivalentTo(expectedRequisites.Name);
			requisites.FullName.Should().BeEquivalentTo(expectedRequisites.FullName);
			requisites.PostalAddress.Should().BeEquivalentTo(expectedRequisites.PostalAddress);
			requisites.LegalAddress.Should().BeEquivalentTo(expectedRequisites.LegalAddress);
			requisites.OKATO.Should().BeEquivalentTo(expectedRequisites.OKATO);
			requisites.INN.Should().BeEquivalentTo(expectedRequisites.INN);
			requisites.OGRN.Should().BeEquivalentTo(expectedRequisites.OGRN);
			requisites.OKPO.Should().BeEquivalentTo(expectedRequisites.OKPO);
			requisites.KPP.Should().BeEquivalentTo(expectedRequisites.KPP);
		}

		private static void CompareBankRequisites(CompanyBankRequisites requisites, 
			CompanyBankRequisites expectedRequisites)
		{
			requisites.AccountNumber.Should().BeEquivalentTo(expectedRequisites.AccountNumber);
			requisites.CorrespondentAccount.Should().BeEquivalentTo(expectedRequisites.CorrespondentAccount);
			requisites.BIC.Should().BeEquivalentTo(expectedRequisites.BIC);
			requisites.Bank.Should().BeEquivalentTo(expectedRequisites.Bank);
		}
	}
}

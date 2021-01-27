using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

using MedStat.Core.DAL;
using MedStat.Core.Identity;
using MedStat.Core.BE.Company;
using MedStat.Core.Info.Company;
using MedStat.Core.Interfaces;

namespace MedStat.Core.Tests.Repositories
{
	public class CompanyUserRepositoryTests : BaseRepositoryTests, IClassFixture<DatabaseFixture>
	{
		public CompanyUserRepositoryTests(ITestOutputHelper outputHelper, DatabaseFixture fixture)
			: base(outputHelper, fixture)
		{
			// Some tests require initialized Roles
			this.InitRolesIfRequired();
		}


		#region Get

		[Fact]
		public void GetCompanyUserInfoAsync_checkUser()
		{
			// Arrange:

			var expectedCmpUser = DataHelper.GetCompanyUserData(true);

			this.AddNewCompanyToDb(expectedCmpUser);


			// Act:

			CompanyUserInfo cmpUserInfo;
			using (var dbContext = new MedStatDbContext(this.Fixture.ContextOptions))
			{
				var sp = this.GetServiceProvider(dbContext);
				var cmpUserRepository = sp.GetRequiredService<ICompanyUserRepository>();

				cmpUserInfo = cmpUserRepository.GetCompanyUserInfoAsync(expectedCmpUser.Id).Result;
			}


			// Assert:

			cmpUserInfo.Should().NotBeNull();
			
			cmpUserInfo.User.Should().NotBeNull();
			cmpUserInfo.User.Description.Should().BeEquivalentTo(expectedCmpUser.Description);

			cmpUserInfo.User.Login.Should().NotBeNull();
			cmpUserInfo.User.Login.Id.Should().Be(expectedCmpUser.Login.Id);
		}

		[Theory]
		[InlineData(false)]
		[InlineData(true)]
		public void GetCompanyUserInfoAsync_checkRoles(bool isPowerUser)
		{
			// Arrange:

			var cmpUserLoginId = this.AddSystemUserToDbAsync(DataHelper.GetSystemUserNewData(),
				new[] { isPowerUser ? UserRoles.CompanyPowerUser : UserRoles.CompanyUser}).Result;
			
			var cmpUser = DataHelper.GetCompanyUserData(false);
			cmpUser.SystemUserId = cmpUserLoginId;

			this.AddNewCompanyToDb(cmpUser);


			// Act:

			CompanyUserInfo cmpUserInfo;
			using (var dbContext = new MedStatDbContext(this.Fixture.ContextOptions))
			{
				var sp = this.GetServiceProvider(dbContext);
				var cmpUserRepository = sp.GetRequiredService<ICompanyUserRepository>();

				cmpUserInfo = cmpUserRepository.GetCompanyUserInfoAsync(cmpUser.Id).Result;
			}


			// Assert:

			cmpUserInfo.Should().NotBeNull();
			cmpUserInfo.IsPowerUser.Should().Be(isPowerUser);
		}

		#endregion


		#region Create / Update / Delete

		[Theory]
		[MemberData(nameof(CreateCompanyUserAsync_Data))]
		public async void CreateCompanyUserAsync(Company companyData,
			string cmpUserDescription, SystemUser cmpUserLogin, bool isPowerUser, 
			bool isValidUserData, Type exceptionType)
		{
			// Arrange:

			int companyId = -1; // non existed company

			if (companyData != null)
			{
				using (var context = new MedStatDbContext(this.Fixture.ContextOptions))
				{
					context.Companies.Add(companyData);
					context.SaveChanges();
				}

				companyId = companyData.Id;
			}


			// Act:

			int? cmpUserId;
			using (var dbContext = new MedStatDbContext(this.Fixture.ContextOptions))
			{
				var sp = this.GetServiceProvider(dbContext);
				var cmpUserRepository = sp.GetRequiredService<ICompanyUserRepository>();

				if (isValidUserData)
				{
					cmpUserId = cmpUserRepository.CreateCompanyUserAsync(companyId,
						cmpUserDescription, cmpUserLogin, isPowerUser).Result;
				}
				else
				{
					// act + assert
					await Assert.ThrowsAsync(exceptionType, () =>
						cmpUserRepository.CreateCompanyUserAsync(companyId, cmpUserDescription, cmpUserLogin, isPowerUser));

					return;
				}
			}


			// Assert:

			cmpUserId.Should().NotBeNull();

			using (var dbContext = new MedStatDbContext(this.Fixture.ContextOptions))
			{
				var dbCmpUser = dbContext.CompanyUsers
					.Include(cu => cu.Login)
					.FirstOrDefault(cu => cu.Id == cmpUserId.Value);

				// Check CompanyUser
				dbCmpUser.Should().NotBeNull();
				dbCmpUser.Description.Should().BeEquivalentTo(cmpUserDescription);
				dbCmpUser.CompanyId.Should().Be(companyId);

				// Partial check of created SystemUser. 
				// Full check for SystemUser creation done in the IdentityRepositoryTests.
				dbCmpUser.Login.Should().NotBeNull();
				dbCmpUser.Login.PhoneNumber.Should().BeEquivalentTo(cmpUserLogin.PhoneNumber);
				
				// Check Rights
				this.CheckUserRoles(dbCmpUser.Login.Id, 
					new []{ isPowerUser ? UserRoles.CompanyPowerUser : UserRoles.CompanyUser });
			}
		}

		[Theory]
		[MemberData(nameof(UpdateCompanyUserAsync_Data))]
		public async void UpdateCompanyUserAsync(CompanyUser existedCmpUserData,
			string cmpUserDescription, SystemUser cmpUserLogin, bool isPowerUser,
			bool isValidUserData, Type exceptionType)
		{
			#region Arrange

			int cmpUserId = -1; // non existed cmp. user

			if (existedCmpUserData != null)
			{
				int systemUserId = this.AddSystemUserToDbAsync(existedCmpUserData.Login,
					// add opposite right for check right update functionality
					new[] {isPowerUser ? UserRoles.CompanyUser : UserRoles.CompanyPowerUser}).Result;
				
				existedCmpUserData.Login = null;
				existedCmpUserData.SystemUserId = systemUserId;
				
				this.AddNewCompanyToDb(existedCmpUserData);

				cmpUserId = existedCmpUserData.Id;
			}

			#endregion

			// Act:

			using (var dbContext = new MedStatDbContext(this.Fixture.ContextOptions))
			{
				var sp = this.GetServiceProvider(dbContext);
				var cmpUserRepository = sp.GetRequiredService<ICompanyUserRepository>();

				if (isValidUserData)
				{
					await cmpUserRepository.UpdateCompanyUserAsync(cmpUserId, cmpUserDescription, cmpUserLogin, isPowerUser);
				}
				else
				{
					// act + assert
					await Assert.ThrowsAsync(exceptionType, () =>
						cmpUserRepository.UpdateCompanyUserAsync(cmpUserId, cmpUserDescription, cmpUserLogin, isPowerUser));

					return;
				}
			}


			// Assert:

			using (var dbContext = new MedStatDbContext(this.Fixture.ContextOptions))
			{
				var dbCmpUser = dbContext.CompanyUsers
					.Include(cu => cu.Login)
					.FirstOrDefault(cu => cu.Id == cmpUserId);

				// Check CompanyUser
				dbCmpUser.Description.Should().BeEquivalentTo(cmpUserDescription);
				
				// Check Login update. 
				dbCmpUser.Login.FirstName.Should().BeEquivalentTo(cmpUserLogin.FirstName);
				dbCmpUser.Login.Surname.Should().BeEquivalentTo(cmpUserLogin.Surname);
				dbCmpUser.Login.Patronymic.Should().BeEquivalentTo(cmpUserLogin.Patronymic);

				// Check Rights
				this.CheckUserRoles(dbCmpUser.Login.Id,
					new[] { isPowerUser ? UserRoles.CompanyPowerUser : UserRoles.CompanyUser });
			}
		}

		[Fact]
		public async void DeleteCompanyUserAsync()
		{
			// Arrange:

			int cmpUserId; 
			{
				int systemUserId = await this.AddSystemUserToDbAsync(DataHelper.GetSystemUserNewData(),
					new[] { UserRoles.CompanyUser });

				var cmpUser = DataHelper.GetCompanyUserData(false);
				cmpUser.SystemUserId = systemUserId;

				this.AddNewCompanyToDb(cmpUser);

				cmpUserId = cmpUser.Id;
			}


			// Act:

			using (var dbContext = new MedStatDbContext(this.Fixture.ContextOptions))
			{
				var sp = this.GetServiceProvider(dbContext);
				var cmpUserRepository = sp.GetRequiredService<ICompanyUserRepository>();

				await cmpUserRepository.DeleteCompanyUserAsync(cmpUserId);

				// act + assert: Try to delete non existed user
				await Assert.ThrowsAsync<OperationCanceledException>(() =>
					cmpUserRepository.DeleteCompanyUserAsync(cmpUserId));

				// Identity errors at deleting SystemUser can be tested in IdentityRepositoryTests
			}


			// Assert:

			using (var dbContext = new MedStatDbContext(this.Fixture.ContextOptions))
			{
				bool isCmpUserExist = dbContext.CompanyUsers.Any(cu => cu.Id == cmpUserId);
				isCmpUserExist.Should().BeFalse();
			}
		}

		#endregion


		// TODO: Tests for FindCompanyUsersAsync


		// Data for Test methods:

		public static IEnumerable<object[]> CreateCompanyUserAsync_Data
		{
			get
			{
				var companyWithExistedCmpUsers = DataHelper.GetCompanyData();
				companyWithExistedCmpUsers.Users = new List<CompanyUser> {  DataHelper.GetCompanyUserData(true) };

				return
					new List<object[]>
					{
						// companyData, cmpUserDescription, cmpUserLogin, isPowerUser
						//	isValidUserData, exceptionType

						// Valid data:

						new object[] { DataHelper.GetCompanyData(), null, DataHelper.GetSystemUserNewData(), false, 
							true, null },
						new object[] { DataHelper.GetCompanyData(), "power user", DataHelper.GetSystemUserNewData(), true,
							true, null },
						new object[] { companyWithExistedCmpUsers, "power user", DataHelper.GetSystemUserNewData(), true,
							true, null },


						// Invalid data:

						// nullable login
						new object[] { DataHelper.GetCompanyData(), "some desc", null, false,
							false, typeof(ArgumentNullException) },
						// Non existed Company
						new object[] { null, "some desc", DataHelper.GetSystemUserNewData(), false,
							false, typeof(OperationCanceledException) }
					};
			}
		}

		public static IEnumerable<object[]> UpdateCompanyUserAsync_Data
		{
			get
			{
				var invalidLoginData1 = DataHelper.GetSystemUserNewData();
				invalidLoginData1.FirstName = "";

				var invalidLoginData2 = DataHelper.GetSystemUserNewData();
				invalidLoginData2.Surname = "";

				return
					new List<object[]>
					{
						// existedCmpUserData, cmpUserDescription, cmpUserLogin, isPowerUser
						//	isValidUserData, exceptionType

						// Valid data:

						new object[] { DataHelper.GetCompanyUserData(true), null, DataHelper.GetSystemUserNewData(), false,
							true, null },
						new object[] { DataHelper.GetCompanyUserData(true), "power user", DataHelper.GetSystemUserNewData(), true,
							true, null },


						// Invalid data:

						// nullable login property
						new object[] { DataHelper.GetCompanyUserData(true), "some desc", invalidLoginData1, false,
							false, typeof(ArgumentNullException) },
						new object[] { DataHelper.GetCompanyUserData(true), "some desc", invalidLoginData2, false,
							false, typeof(ArgumentNullException) },
						// Non existed CompanyUser
						new object[] { null, "some desc", DataHelper.GetSystemUserNewData(), false,
							false, typeof(OperationCanceledException) }
					};
			}
		}


		// Helpers:

		private void AddNewCompanyToDb(params CompanyUser[] cmpUserData)
		{
			using (var context = new MedStatDbContext(this.Fixture.ContextOptions))
			{
				var company = DataHelper.GetCompanyData();
				company.Users = new List<CompanyUser>();
				company.Users.AddRange(cmpUserData);

				context.Companies.Add(company);
				context.SaveChanges();
			}
		}
	}
}

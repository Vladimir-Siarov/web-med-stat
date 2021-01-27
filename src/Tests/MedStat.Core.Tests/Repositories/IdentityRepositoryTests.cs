using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;
using Xunit.Abstractions;

using MedStat.Core.DAL;
using MedStat.Core.Helpers;
using MedStat.Core.Identity;
using MedStat.Core.Interfaces;

namespace MedStat.Core.Tests.Repositories
{
	public class IdentityRepositoryTests : BaseRepositoryTests, IClassFixture<DatabaseFixture>
	{
		public IdentityRepositoryTests(ITestOutputHelper outputHelper, DatabaseFixture fixture)
			: base(outputHelper, fixture)
		{
			// Some tests require initialized Roles
			this.InitRolesIfRequired();
		}


		#region SystemUser
		
		[Fact]
		public async void FindByPhoneNumberAsync()
		{
			// Arrange:

			var expectedUser = DataHelper.GetSystemUserNewData();
			
			await this.AddSystemUserToDbAsync(expectedUser);
			await this.AddSystemUserToDbAsync(DataHelper.GetSystemUserNewData());
			await this.AddSystemUserToDbAsync(DataHelper.GetSystemUserNewData());


			// Act:

			SystemUser user;
			using (var dbContext = new MedStatDbContext(this.Fixture.ContextOptions))
			{
				var sp = this.GetServiceProvider(dbContext);
				var identityRepository = sp.GetRequiredService<IIdentityRepository>();

				user = identityRepository.FindByPhoneNumberAsync(expectedUser.PhoneNumber).Result;
			}


			// Assert:

			user.Should().NotBeNull();
			
			user.Id.Should().Be(expectedUser.Id);
			user.PhoneNumber.Should().BeEquivalentTo(expectedUser.PhoneNumber);
			
			user.FirstName.Should().Be(expectedUser.FirstName);
			user.Surname.Should().Be(expectedUser.Surname);
			user.Patronymic.Should().Be(expectedUser.Patronymic);
		}

		[Theory]
		[MemberData(nameof(CreateSystemUserByPhoneNumberAsync_UnderOuterTransaction_Data))]
		public async void CreateSystemUserByPhoneNumberAsync_UnderOuterTransaction(SystemUser userData,
			IEnumerable<string> userRoles, bool isValidUserData, Type exceptionType)
		{
			// Arrange:
			

			// Act:

			int? userId;
			using (var dbContext = new MedStatDbContext(this.Fixture.ContextOptions))
			{
				var sp = this.GetServiceProvider(dbContext);
				var identityRepo = sp.GetRequiredService<IIdentityRepository>();

				if (isValidUserData)
				{
					userId = identityRepo.CreateSystemUserByPhoneNumberAsync_UnderOuterTransaction(userData, userRoles)
						.Result?.Id;
				}
				else
				{
					// act + assert
					await Assert.ThrowsAsync(exceptionType, () =>
						identityRepo.CreateSystemUserByPhoneNumberAsync_UnderOuterTransaction(userData, userRoles));

					return;
				}
			}


			// Assert:

			userId.Should().NotBeNull();

			using (var dbContext = new MedStatDbContext(this.Fixture.ContextOptions))
			{
				var dbUser = dbContext.SystemUsers.FirstOrDefault(su => su.Id == userId.Value);

				dbUser.Should().NotBeNull();
				dbUser.PhoneNumber.Should().BeEquivalentTo(userData.PhoneNumber);
				dbUser.NormalizedPhoneNumber.Should().NotBeNullOrEmpty();
			}

			// Check user Roles
			if (userRoles != null)
			{
				this.CheckUserRoles(userId.Value, userRoles);
			}
		}

		[Fact]
		public async void DeleteSystemUserAsync_UnderOuterTransaction()
		{
			// Arrange:

			int userId = await this.AddSystemUserToDbAsync(DataHelper.GetSystemUserNewData());
			

			// Act:

			using (var dbContext = new MedStatDbContext(this.Fixture.ContextOptions))
			{
				var sp = this.GetServiceProvider(dbContext);
				var identityRepo = sp.GetRequiredService<IIdentityRepository>();

				await identityRepo.DeleteSystemUserAsync_UnderOuterTransaction(userId);
			}


			// Assert:

			using (var dbContext = new MedStatDbContext(this.Fixture.ContextOptions))
			{
				bool isUserExist = dbContext.SystemUsers.Any(su => su.Id == userId);
				isUserExist.Should().BeFalse();
			}
		}

		// Comment: It's an example how we can test cases with "IdentityResult.Failed" results.
		// But we more interesting in testing business logic and in integration tests.
		[Fact]
		public async void DeleteSystemUserAsync_UnderOuterTransaction_IdentityResultException()
		{
			// Arrange:

			var storeMock = new Mock<IUserStore<SystemUser>>();
			storeMock
				.Setup(m => m.DeleteAsync(It.IsAny<SystemUser>(), It.IsAny<CancellationToken>()))
				.Returns(() => Task.FromResult(
					IdentityResult.Failed(new IdentityError { Description = "Some delete SystemUser error" })));

			var userManager = new UserManager<SystemUser>(storeMock.Object, 
				null, null, null, null, null, null, null, null);

			// add new user to DB by using standard UserManger
			int userId = await this.AddSystemUserToDbAsync(DataHelper.GetSystemUserNewData());
			
			
			// Act + Assert
			using (var dbContext = new MedStatDbContext(this.Fixture.ContextOptions))
			{
				var sp = this.GetServiceProvider(dbContext, userManager);
				var identityRepo = sp.GetRequiredService<IIdentityRepository>();

				await Assert.ThrowsAsync<IdentityResultException>(() => 
					identityRepo.DeleteSystemUserAsync_UnderOuterTransaction(userId));
			}
		}

		#endregion


		#region Roles

		[Theory]
		[MemberData(nameof(AddToRolesAsync_Data))]
		public async void AddToRolesAsync(IEnumerable<string> userRoles,
			IEnumerable<string> newRoles, bool checkBeforeAdding, 
			Type exceptionType)
		{
			// Arrange:

			int userId = await this.AddSystemUserToDbAsync(DataHelper.GetSystemUserNewData(), userRoles);
			
			
			// Act:

			using (var dbContext = new MedStatDbContext(this.Fixture.ContextOptions))
			{
				var sp = this.GetServiceProvider(dbContext);
				var identityRepo = sp.GetRequiredService<IIdentityRepository>();
				var user = dbContext.SystemUsers.First(u => u.Id == userId);

				if (exceptionType == null)
				{
					await identityRepo.AddToRolesAsync(user, newRoles, checkBeforeAdding);
				}
				else
				{
					// act + assert
					await Assert.ThrowsAsync(exceptionType, () =>
						identityRepo.AddToRolesAsync(user, newRoles, checkBeforeAdding));

					return;
				}
			}


			// Assert:

			this.CheckUserRoles(userId, newRoles);
		}

		[Theory]
		[MemberData(nameof(RemoveFromRolesAsync_Data))]
		public async void RemoveFromRolesAsync(IEnumerable<string> userRoles,
			IEnumerable<string> rolesForRemove, bool checkBeforeRemoving,
			Type exceptionType)
		{
			// Arrange:

			int userId = await this.AddSystemUserToDbAsync(DataHelper.GetSystemUserNewData(), userRoles);


			// Act:

			using (var dbContext = new MedStatDbContext(this.Fixture.ContextOptions))
			{
				var sp = this.GetServiceProvider(dbContext);
				var identityRepo = sp.GetRequiredService<IIdentityRepository>();
				var user = dbContext.SystemUsers.First(u => u.Id == userId);

				if (exceptionType == null)
				{
					await identityRepo.RemoveFromRolesAsync(user, rolesForRemove, checkBeforeRemoving);
				}
				else
				{
					// act + assert
					await Assert.ThrowsAsync(exceptionType, () =>
						identityRepo.RemoveFromRolesAsync(user, rolesForRemove, checkBeforeRemoving));

					return;
				}
			}


			// Assert:

			this.CheckUserRoles(userId, rolesForRemove, checkIsNotInExpectedRoles: true);
		}

		[Theory]
		[MemberData(nameof(GetUserRolesAsync_Data))]
		public async void GetUserRolesAsync(IEnumerable<string> userRoles)
		{
			// Arrange:

			int userId = await this.AddSystemUserToDbAsync(DataHelper.GetSystemUserNewData(), userRoles);


			// Act:

			IEnumerable<string> testedRoles;
			using (var dbContext = new MedStatDbContext(this.Fixture.ContextOptions))
			{
				var sp = this.GetServiceProvider(dbContext);
				var identityRepo = sp.GetRequiredService<IIdentityRepository>();
				var user = dbContext.SystemUsers.First(u => u.Id == userId);

				testedRoles = await identityRepo.GetUserRolesAsync(user);
			}


			// Assert:

			testedRoles.Should().NotBeNull();
			Assert.True(testedRoles.Count() == userRoles.Count());
			foreach (string testedRole in testedRoles)
			{
				Assert.Contains(testedRole, userRoles);
			}
		}

		#endregion


		#region Phone number

		[Theory]
		[MemberData(nameof(ChangeUserPhoneNumberAsync_Data))]
		public async void ChangeUserPhoneNumberAsync(SystemUser[] userDataArray,
			string phoneNumber, string newPhoneNumber,
			Type exceptionType)
		{
			// Arrange:

			if (userDataArray != null)
			{
				foreach (var userData in userDataArray)
				{
					await this.AddSystemUserToDbAsync(userData);
				}
			}


			// Act:

			using (var dbContext = new MedStatDbContext(this.Fixture.ContextOptions))
			{
				var sp = this.GetServiceProvider(dbContext);
				var identityRepo = sp.GetRequiredService<IIdentityRepository>();
				
				if (exceptionType == null)
				{
					await identityRepo.ChangeUserPhoneNumberAsync(phoneNumber, newPhoneNumber);
				}
				else
				{
					// act + assert
					await Assert.ThrowsAsync(exceptionType, () =>
						identityRepo.ChangeUserPhoneNumberAsync(phoneNumber, newPhoneNumber));

					return;
				}
			}


			// Assert:

			using (var dbContext = new MedStatDbContext(this.Fixture.ContextOptions))
			{
				var user = dbContext.SystemUsers.FirstOrDefault(u => u.PhoneNumber == newPhoneNumber);

				user.Should().NotBeNull();

				var normalizedNewPhoneNumber = PhoneHelper.NormalizePhoneNumber(newPhoneNumber);
				user.NormalizedPhoneNumber.Should().BeEquivalentTo(normalizedNewPhoneNumber);

				// check custom business logic
				user.UserName.Should().BeEquivalentTo(normalizedNewPhoneNumber);
				user.IsPasswordChangeRequired.Should().BeTrue();
			}
		}

		#endregion


		#region Password

		[Theory]
		[MemberData(nameof(SetPasswordChangeRequiredFlagAsync_Data))]
		public async void SetPasswordChangeRequiredFlagAsync(SystemUser userData,
			string phoneNumber, bool isChangePasswordRequired,
			Type exceptionType)
		{
			// Arrange:

			if (userData != null)
			{
				await this.AddSystemUserToDbAsync(userData);
			}


			// Act:

			using (var dbContext = new MedStatDbContext(this.Fixture.ContextOptions))
			{
				var sp = this.GetServiceProvider(dbContext);
				var identityRepo = sp.GetRequiredService<IIdentityRepository>();

				if (exceptionType == null)
				{
					await identityRepo.SetPasswordChangeRequiredFlagAsync(phoneNumber, isChangePasswordRequired);
				}
				else
				{
					// act + assert
					await Assert.ThrowsAsync(exceptionType, () =>
						identityRepo.SetPasswordChangeRequiredFlagAsync(phoneNumber, isChangePasswordRequired));

					return;
				}
			}


			// Assert:

			using (var dbContext = new MedStatDbContext(this.Fixture.ContextOptions))
			{
				var user = dbContext.SystemUsers.FirstOrDefault(u => u.PhoneNumber == phoneNumber);

				user.Should().NotBeNull();
				user.IsPasswordChangeRequired.Should().Be(isChangePasswordRequired);
			}
		}

		[Theory]
		[MemberData(nameof(ChangeUserPasswordAsync_Data))]
		public async void ChangeUserPasswordAsync(SystemUser userData, string userPassword,
			string phoneNumber, string newPassword,
			Type exceptionType)
		{
			// Arrange:

			if (userData != null)
			{
				await this.AddSystemUserToDbAsync(userData, null, userPassword);
			}


			// Act:

			using (var dbContext = new MedStatDbContext(this.Fixture.ContextOptions))
			{
				var sp = this.GetServiceProvider(dbContext);
				var identityRepo = sp.GetRequiredService<IIdentityRepository>();

				if (exceptionType == null)
				{
					await identityRepo.ChangeUserPasswordAsync(phoneNumber, newPassword);
				}
				else
				{
					// act + assert
					await Assert.ThrowsAsync(exceptionType, () =>
						identityRepo.ChangeUserPasswordAsync(phoneNumber, newPassword));

					return;
				}
			}


			// Assert:

			using (var dbContext = new MedStatDbContext(this.Fixture.ContextOptions))
			{
				var sp = this.GetServiceProvider(dbContext);
				var userManager = sp.GetRequiredService<UserManager<SystemUser>>();

				var user = dbContext.SystemUsers.FirstOrDefault(u => u.PhoneNumber == phoneNumber);
				user.Should().NotBeNull();

				bool passwordWasChanged = await userManager.CheckPasswordAsync(user, newPassword);
				passwordWasChanged.Should().BeTrue();
			}
		}

		#endregion


		// Data for Test methods:

		public static IEnumerable<object[]> CreateSystemUserByPhoneNumberAsync_UnderOuterTransaction_Data
		{
			get
			{
				var invalidUserData1 = DataHelper.GetSystemUserNewData();
				invalidUserData1.PhoneNumber = null;

				var invalidUserData2 = DataHelper.GetSystemUserNewData();
				invalidUserData2.FirstName = null;

				var invalidUserData3 = DataHelper.GetSystemUserNewData();
				invalidUserData3.Surname = null;

				var userData1 = DataHelper.GetSystemUserNewData();
				userData1.NormalizedPhoneNumber = null;
				var userRights1 = new string[] { UserRoles.CompanyUser };

				var userData2 = DataHelper.GetSystemUserNewData();
				var userRights2 = new string[] { UserRoles.CompanyPowerUser };

				// User with duplicated phone number
				var invalidUserData4 = DataHelper.GetSystemUserNewData();
				invalidUserData4.PhoneNumber = userData2.PhoneNumber;
				invalidUserData4.NormalizedPhoneNumber = null;

				return
					new List<object[]>
					{
						// userData, userRoles, isValidUserData, exceptionType

						new object[] { invalidUserData1, null, false, typeof(ArgumentNullException) },
						new object[] { invalidUserData2, null, false, typeof(ArgumentNullException) },
						new object[] { invalidUserData3, null, false, typeof(ArgumentNullException) },

						new object[] { DataHelper.GetSystemUserNewData(), null, true, null },

						new object[] { userData1, userRights1, true, null },
						new object[] { userData2, userRights2, true, null },

						new object[] { invalidUserData4, null, false, typeof(OperationCanceledException) }
					};
			}
		}


		public static IEnumerable<object[]> AddToRolesAsync_Data
		{
			get
			{
				return
					new List<object[]>
					{
						// userRoles, newRoles, checkBeforeAdding, exceptionType

						// Valid data:

						new object[] { null, new string[0], false, null },
						new object[] { null, new[] { UserRoles.CompanyUser, UserRoles.CompanyPowerUser }, false, null },
						// intersect roles but checking before adding
						new object[] { new[] { UserRoles.CompanyUser }, 
							new[] { UserRoles.CompanyUser, UserRoles.CompanyPowerUser }, true, null },


						// Invalid data:

						// invalid (nullable) new roles
						new object[] { null, null, false, typeof(ArgumentNullException) },
						// try to add non-existent role (checking before adding doesn't help)
						new object[] { null, new[] { "Some non-existent role" }, true, typeof(InvalidOperationException) },
						// intersect roles without checking before adding
						new object[] { new[] { UserRoles.CompanyUser },
							new[] { UserRoles.CompanyUser, UserRoles.CompanyPowerUser }, false, typeof(IdentityResultException) }
					};
			}
		}

		public static IEnumerable<object[]> GetUserRolesAsync_Data
		{
			get
			{
				return
					new List<object[]>
					{
						// userRoles

						// Valid data:

						new object[] { new string[0] },
						new object[] { new[] { UserRoles.CompanyUser } },
						new object[] { new[] { UserRoles.CompanyUser, UserRoles.CompanyPowerUser, UserRoles.SystemAdmin } }
					};
			}
		}

		public static IEnumerable<object[]> RemoveFromRolesAsync_Data
		{
			get
			{
				return
					new List<object[]>
					{
						// userRoles, rolesForRemove, checkBeforeRemoving, exceptionType

						// Valid data:

						new object[] { null, new string[0], false, null },
						new object[] { new[] { UserRoles.CompanyUser, UserRoles.CompanyPowerUser },
							new[] { UserRoles.CompanyPowerUser }, false, null },
						// try to remove non-existent role, but check it before deleting
						new object[] { null, new[] { "Some non-existent role" }, true, null },
						// try to remove roles which is not assigned to the user, but check it before deleting
						new object[] { new[] { UserRoles.CompanyUser },
							new[] { UserRoles.CompanyUser, UserRoles.CompanyPowerUser }, true, null },


						// Invalid data:

						// invalid (nullable) roles for remove
						new object[] { null, null, false, typeof(ArgumentNullException) },
						// try to remove non-existent role, without check it before deleting
						new object[] { null, new[] { "Some non-existent role" }, false, typeof(IdentityResultException) },
						// try to remove roles which is not assigned to the user, without check it before deleting
						new object[] { new[] { UserRoles.CompanyUser },
							new[] { UserRoles.CompanyUser, UserRoles.CompanyPowerUser }, false, typeof(IdentityResultException) },
					};
			}
		}


		public static IEnumerable<object[]> ChangeUserPhoneNumberAsync_Data
		{
			get
			{
				var userData1 = DataHelper.GetSystemUserNewData();
				var userData2 = DataHelper.GetSystemUserNewData();
				var userData3_1 = DataHelper.GetSystemUserNewData();
				var userData3_2 = DataHelper.GetSystemUserNewData();

				return
					new List<object[]>
					{
						// userDataArray, phoneNumber, newPhoneNumber, exceptionType

						// Valid:
						new object[] { new[] { userData1 }, userData1.PhoneNumber, userData1.PhoneNumber.Replace("+7", "+6"), null },
						
						// Invalid:
						// Invalid phone numbers arguments
						new object[] { null, "", "+5 911 111-11-11", typeof(ArgumentNullException) },
						new object[] { null, "+5 911 111-11-11", "", typeof(ArgumentNullException) },
						// User with specified phone number doesn't exist
						new object[] { null, "+1 911 111-11-11", "+1 911 222-22-22", typeof(OperationCanceledException) },
						// Phone number the same (is not unique)
						new object[] { new[] { userData2 }, userData2.PhoneNumber, userData2.PhoneNumber, 
							typeof(OperationCanceledException) },
						// Phone number is not unique
						new object[] { new[] { userData3_1, userData3_2 }, userData3_2.PhoneNumber, userData3_1.PhoneNumber, 
							typeof(OperationCanceledException) }
					};
			}
		}


		public static IEnumerable<object[]> SetPasswordChangeRequiredFlagAsync_Data
		{
			get
			{
				var userData1 = DataHelper.GetSystemUserNewData();
				userData1.IsPasswordChangeRequired = false;

				var userData2 = DataHelper.GetSystemUserNewData();
				userData2.IsPasswordChangeRequired = true;

				return
					new List<object[]>
					{
						// userData, phoneNumber, isChangePasswordRequired, exceptionType

						// Valid:
						new object[] { userData1, userData1.PhoneNumber, !userData1.IsPasswordChangeRequired, null },
						new object[] { userData2, userData2.PhoneNumber, !userData2.IsPasswordChangeRequired, null },

						// Invalid:
						// Invalid phone number argument
						new object[] { null, "", true, typeof(ArgumentNullException) },
						// User with specified phone number doesn't exist
						new object[] { null, "+1 911 111-11-11", true, typeof(OperationCanceledException) }
					};
			}
		}

		public static IEnumerable<object[]> ChangeUserPasswordAsync_Data
		{
			get
			{
				var userData1 = DataHelper.GetSystemUserNewData();
				var userData2 = DataHelper.GetSystemUserNewData();
				var userData3 = DataHelper.GetSystemUserNewData();

				return
					new List<object[]>
					{
						// userData, userPassword,
						//	phoneNumber, newPassword, exceptionType

						// Valid:
						// User without password
						new object[] { userData1, null,
							userData1.PhoneNumber, DataHelper.GenerateUserPassword(), null },
						// User with existed password
						new object[] { userData2, DataHelper.GenerateUserPassword(),
							userData2.PhoneNumber, DataHelper.GenerateUserPassword(), null },

						// Invalid:
						// Invalid phone number argument
						new object[] { null, null,
							"", DataHelper.GenerateUserPassword(), typeof(ArgumentNullException) },
						// Invalid new password argument
						new object[] { userData3, null,
							userData3.PhoneNumber, "", typeof(ArgumentNullException) },
						// User with specified phone number doesn't exist
						new object[] { null, null,
							"+1 911 111-11-11", DataHelper.GenerateUserPassword(), typeof(OperationCanceledException) }
					};
			}
		}


		// Helpers:
	}
}

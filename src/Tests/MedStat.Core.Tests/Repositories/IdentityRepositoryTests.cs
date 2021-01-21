using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
		private static long _phoneNumValue = 1111111;
		private DatabaseFixture _fixture;
		

		public IdentityRepositoryTests(DatabaseFixture fixture, ITestOutputHelper outputHelper)
			: base(outputHelper)
		{
			this._fixture = fixture;

			// Some tests require initialized Roles
			using (var dbContext = new MedStatDbContext(_fixture.ContextOptions))
			{
				Task.Run(() => this.InitRolesAsync(dbContext)).Wait();
			}
		}


		#region SystemUser
		
		[Fact]
		public async void FindByPhoneNumberAsync()
		{
			// Arrange:

			var expectedUser = GetSystemUserNewData();
			
			await this.AddSystemUserToDbAsync(expectedUser);
			await this.AddSystemUserToDbAsync(GetSystemUserNewData());
			await this.AddSystemUserToDbAsync(GetSystemUserNewData());


			// Act:

			SystemUser user;
			using (var dbContext = new MedStatDbContext(_fixture.ContextOptions))
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
			using (var dbContext = new MedStatDbContext(_fixture.ContextOptions))
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

			using (var dbContext = new MedStatDbContext(_fixture.ContextOptions))
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

			int userId = await this.AddSystemUserToDbAsync(GetSystemUserNewData());
			

			// Act:

			using (var dbContext = new MedStatDbContext(_fixture.ContextOptions))
			{
				var sp = this.GetServiceProvider(dbContext);
				var identityRepo = sp.GetRequiredService<IIdentityRepository>();

				await identityRepo.DeleteSystemUserAsync_UnderOuterTransaction(userId);
			}


			// Assert:

			using (var dbContext = new MedStatDbContext(_fixture.ContextOptions))
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
			int userId = await this.AddSystemUserToDbAsync(GetSystemUserNewData());
			
			
			// Act + Assert
			using (var dbContext = new MedStatDbContext(_fixture.ContextOptions))
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

			int userId = await this.AddSystemUserToDbAsync(GetSystemUserNewData(), userRoles);
			
			
			// Act:

			using (var dbContext = new MedStatDbContext(_fixture.ContextOptions))
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

			int userId = await this.AddSystemUserToDbAsync(GetSystemUserNewData(), userRoles);


			// Act:

			using (var dbContext = new MedStatDbContext(_fixture.ContextOptions))
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

			int userId = await this.AddSystemUserToDbAsync(GetSystemUserNewData(), userRoles);


			// Act:

			IEnumerable<string> testedRoles;
			using (var dbContext = new MedStatDbContext(_fixture.ContextOptions))
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


		// Data for Test methods:

		public static IEnumerable<object[]> CreateSystemUserByPhoneNumberAsync_UnderOuterTransaction_Data
		{
			get
			{
				var invalidUserData1 = GetSystemUserNewData();
				invalidUserData1.PhoneNumber = null;

				var invalidUserData2 = GetSystemUserNewData();
				invalidUserData2.FirstName = null;

				var invalidUserData3 = GetSystemUserNewData();
				invalidUserData3.Surname = null;

				var userData1 = GetSystemUserNewData();
				userData1.NormalizedPhoneNumber = null;
				var userRights1 = new string[] { UserRoles.CompanyUser };

				var userData2 = GetSystemUserNewData();
				var userRights2 = new string[] { UserRoles.CompanyPowerUser };

				// User with duplicated phone number
				var invalidUserData4 = GetSystemUserNewData();
				invalidUserData4.PhoneNumber = userData2.PhoneNumber;
				invalidUserData4.NormalizedPhoneNumber = null;

				return
					new List<object[]>
					{
						// userData, userRoles, isValidUserData, exceptionType

						new object[] { invalidUserData1, null, false, typeof(ArgumentNullException) },
						new object[] { invalidUserData2, null, false, typeof(ArgumentNullException) },
						new object[] { invalidUserData3, null, false, typeof(ArgumentNullException) },

						new object[] { GetSystemUserNewData(), null, true, null },

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
							new[] { UserRoles.CompanyUser, UserRoles.CompanyPowerUser }, false, typeof(IdentityResultException) },
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


		// Helpers:

		public static SystemUser GetSystemUserNewData()
		{
			_phoneNumValue += 1;

			var phoneNumber = $"+7 911 {_phoneNumValue:###-##-##}";
			var normalizedPhoneNumber = PhoneHelper.NormalizePhoneNumber(phoneNumber);
			var guid = Guid.NewGuid();

			var user = new SystemUser
			{
				UserName = normalizedPhoneNumber,
				//NormalizedUserName = normalizedPhoneNumber,
				PhoneNumber = phoneNumber,
				NormalizedPhoneNumber = normalizedPhoneNumber,
				FirstName = $"FirstName {guid}",
				Surname = $"Surname {guid}",
				Patronymic = $"Patronymic {guid}"
			};

			return user;
		}


		private void CheckUserRoles(int userId, IEnumerable<string> expectedUserRoles,
			bool checkIsNotInExpectedRoles = false)
		{
			using (var dbContext = new MedStatDbContext(_fixture.ContextOptions))
			{
				var dbUser = dbContext.SystemUsers.FirstOrDefault(su => su.Id == userId);

				dbUser.Should().NotBeNull();

				var sp = this.GetServiceProvider(dbContext);
				var userManager = sp.GetRequiredService<UserManager<SystemUser>>();

				var dbUserRoles = userManager.GetRolesAsync(dbUser).Result;

				if (checkIsNotInExpectedRoles) // check that user NOT IN expected roles
				{
					foreach (string expectedRole in expectedUserRoles)
					{
						Assert.DoesNotContain(expectedRole, dbUserRoles);
					}
				}
				else // check that user IN expected roles
				{
					Assert.True(dbUserRoles.Count == expectedUserRoles.Count());
					foreach (string expectedRole in expectedUserRoles)
					{
						Assert.Contains(expectedRole, dbUserRoles);
					}
				}
			}
		}

		public async Task<int> AddSystemUserToDbAsync(SystemUser userData, 
			IEnumerable<string> userRoles = null)
		{
			using (var dbContext = new MedStatDbContext(_fixture.ContextOptions))
			{
				var sp = this.GetServiceProvider(dbContext);
				var userManager = sp.GetRequiredService<UserManager<SystemUser>>();
				
				var createUserResult = await userManager.CreateAsync(userData);
				if (!createUserResult.Succeeded)
					throw new Exception("Test arrange is failed. Cannot add SystemUser entity to DB.");

				if (userRoles != null)
				{
					var addToRolesResult = await userManager.AddToRolesAsync(userData, userRoles);
					if (!addToRolesResult.Succeeded)
						throw new Exception("Test arrange is failed. Cannot add SystemUser to roles.");
				}

				return 
					userData.Id;
			}
		}
	}
}

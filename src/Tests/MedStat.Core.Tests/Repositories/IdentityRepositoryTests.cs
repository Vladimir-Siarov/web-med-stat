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
		public void FindByPhoneNumberAsync()
		{
			// Arrange:

			var expectedUser = GetSystemUserNewData();

			using (var context = new MedStatDbContext(_fixture.ContextOptions))
			{
				context.SystemUsers.Add(expectedUser);
				context.SystemUsers.Add(GetSystemUserNewData());
				context.SystemUsers.Add(GetSystemUserNewData());
				context.SaveChanges();
			}


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
					_ = await Assert.ThrowsAsync(exceptionType, () =>
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

			int userId;
			using (var dbContext = new MedStatDbContext(_fixture.ContextOptions))
			{
				var userData = GetSystemUserNewData();

				dbContext.SystemUsers.Add(userData);
				dbContext.SaveChanges();

				userId = userData.Id;
			}


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
		// But we more interesting in testing business logic.
		[Fact]
		public async void DeleteSystemUserAsync_UnderOuterTransaction_IdentityResultException()
		{
			// Arrange:

			var storeMock = new Mock<IUserStore<SystemUser>>();
			storeMock
				.Setup(m => m.DeleteAsync(It.IsAny<SystemUser>(), It.IsAny<CancellationToken>()))
				.Returns(() => Task.FromResult(
					IdentityResult.Failed(new IdentityError { Description = "Some delete SystemUser error" })));

			var userManager = new UserManager<SystemUser>(storeMock.Object, null, null, null, null, null, null, null, null);

			int userId;
			using (var dbContext = new MedStatDbContext(_fixture.ContextOptions))
			{
				var userData = GetSystemUserNewData();

				dbContext.SystemUsers.Add(userData);
				dbContext.SaveChanges();

				userId = userData.Id;
			}

			
			using (var dbContext = new MedStatDbContext(_fixture.ContextOptions))
			{
				// Act:
				var sp = this.GetServiceProvider(dbContext, userManager);
				var identityRepo = sp.GetRequiredService<IIdentityRepository>();

				// Assert:
				await Assert.ThrowsAsync<IdentityResultException>(async () => 
					await identityRepo.DeleteSystemUserAsync_UnderOuterTransaction(userId));
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


		// Helpers:

		public static SystemUser GetSystemUserNewData()
		{
			_phoneNumValue += 1;

			var phoneNumber = $"+7 911 {_phoneNumValue:###-##-##}";
			var guid = Guid.NewGuid();

			var user = new SystemUser
			{
				UserName = phoneNumber,
				PhoneNumber = phoneNumber,
				NormalizedPhoneNumber = PhoneHelper.NormalizePhoneNumber(phoneNumber),
				FirstName = $"FirstName {guid}",
				Surname = $"Surname {guid}",
				Patronymic = $"Patronymic {guid}"
			};

			return user;
		}


		private void CheckUserRoles(int userId, IEnumerable<string> expectedUserRoles)
		{
			using (var dbContext = new MedStatDbContext(_fixture.ContextOptions))
			{
				var dbUser = dbContext.SystemUsers.FirstOrDefault(su => su.Id == userId);

				dbUser.Should().NotBeNull();

				var sp = this.GetServiceProvider(dbContext);
				var userManager = sp.GetRequiredService<UserManager<SystemUser>>();

				var dbUserRoles = userManager.GetRolesAsync(dbUser).Result;

				Assert.True(dbUserRoles.Count == expectedUserRoles.Count());
				foreach (string role in expectedUserRoles)
				{
					Assert.True(dbUserRoles.Contains(role));
				}
			}
		}
	}
}

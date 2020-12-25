using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MedStat.Core.DAL;
using MedStat.Core.Identity;
using MedStat.Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace MedStat.Core.Repositories
{
	public class SecurityRepository : BaseRepository, ISecurityRepository
	{
		private readonly IIdentityRepository _identityRepository;


		public SecurityRepository(IIdentityRepository identityRepository,
			MedStatDbContext dbContext, ILogger<SecurityRepository> logger/*, string userUid*/)
			: base(dbContext, logger, null/*userUid*/)
		{
			this._identityRepository = identityRepository;
		}


		public async Task SetupRolesAsync(RoleManager<IdentityRole<Int32>> roleManager)
		{
			// Check and create Roles
			foreach (string role in UserRoles.GetAllRoles())
			{
				if (await roleManager.FindByNameAsync(role) == null)
				{
					await roleManager.CreateAsync(new IdentityRole<int>(role));
					this.Logger.LogInformation("Role {roleName} was successfully added to the system", role);
				}
			}
    }

		public async Task SetupSystemAdminAsync(string adminPhoneNumber, string adminPassword)
		{
			SystemUser admin = await _identityRepository.FindByPhoneNumberAsync(adminPhoneNumber);

			// Create System Admin user
			if (admin == null)
			{
				admin = new SystemUser 
				{ 
					PhoneNumber = adminPhoneNumber, 
					
					FirstName = "Admin",
					Surname = "Adminov"
				};

				await using (var transaction = await this.DbContext.Database.BeginTransactionAsync())
				{
					admin = await _identityRepository.CreateSystemUserByPhoneNumberAsync_UnderOuterTransaction(admin,
						adminPassword, new[] {UserRoles.SystemAdmin});
					
					await transaction.CommitAsync();

					this.Logger.LogInformation("Admin user {@User} was created successfully",
						new { admin.Id, admin.UserName });
					this.Logger.LogInformation("Admin user {@User} was added to the \"{roleName}\" role",
						new { admin.Id, admin.UserName }, UserRoles.SystemAdmin);
				}
			}
			else
			{
				var addedRoles = await _identityRepository.AddToRolesAsync(admin, 
					new[] {UserRoles.SystemAdmin}, true);

				if (addedRoles.Any())
				{
					this.Logger.LogInformation("Admin user {@User} was added to the \"{roleName}\" role",
						new { admin.Id, admin.UserName }, UserRoles.SystemAdmin);
				}
			}
		}
	}
}

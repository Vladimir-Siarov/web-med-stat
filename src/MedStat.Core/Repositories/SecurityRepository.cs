using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MedStat.Core.DAL;
using MedStat.Core.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace MedStat.Core.Repositories
{
	public class SecurityRepository : BaseRepository
	{
		public SecurityRepository(MedStatDbContext dbContext, ILogger<SecurityRepository> logger/*, string userUid*/)
			: base(dbContext, logger, null/*userUid*/)
		{
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

		public async Task SetupSystemAdminAsync(UserManager<SystemUser> userManager,
			string adminEmail, string adminPassword)
		{
			SystemUser admin = await userManager.FindByNameAsync(adminEmail);

			// Check and Create System Admin user
			if (admin == null)
			{
				admin = new SystemUser 
				{ 
					Email = adminEmail, 
					UserName = adminEmail,
					FirstName = "Admin",
					Surname = "Adminov"
				};
				IdentityResult result = await userManager.CreateAsync(admin, adminPassword);

				if (result.Succeeded)
				{
					this.Logger.LogInformation("Admin user {@User} was created successfully",
						new { admin.Id, admin.UserName });
					
					await userManager.AddToRoleAsync(admin, UserRoles.SystemAdmin);
					this.Logger.LogInformation("Admin user {@User} was added to the \"{roleName}\" role",
						new { admin.Id, admin.UserName }, UserRoles.SystemAdmin);
				}
			}
			else
			{
				if (false == await userManager.IsInRoleAsync(admin, UserRoles.SystemAdmin))
				{
					await userManager.AddToRoleAsync(admin, UserRoles.SystemAdmin);
					this.Logger.LogInformation("Admin user {@User} was added to the \"{roleName}\" role",
						new { admin.Id, admin.UserName }, UserRoles.SystemAdmin);
				}
			}
		}
	}
}

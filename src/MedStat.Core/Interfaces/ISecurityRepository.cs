using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace MedStat.Core.Interfaces
{
	public interface ISecurityRepository
	{
		/// <summary>
		/// Adds required roles to the DB.
		/// </summary>
		/// <param name="roleManager"></param>
		/// <returns></returns>
		Task SetupRolesAsync(RoleManager<IdentityRole<Int32>> roleManager);

		/// <summary>
		/// Creates (if required) System Admin user and it to the appropriate roles.
		/// </summary>
		/// <param name="adminPhoneNumber"></param>
		/// <returns></returns>
		Task SetupSystemAdminAsync(string adminPhoneNumber);
	}
}

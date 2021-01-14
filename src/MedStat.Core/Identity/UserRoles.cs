
using System.Collections.Generic;

namespace MedStat.Core.Identity
{
	public static class UserRoles
	{
		public const string SystemAdmin = "system_admin";

		// Company users:
		public const string CompanyUser = "company_user";
		public const string CompanyPowerUser = "company_power_user";


		internal static string[] GetAllRoles()
		{
			return 
				new[]
				{
					SystemAdmin,

					CompanyUser,
					CompanyPowerUser
				};
		}
	}


	public class AccessPolicies
	{
		/// <summary>
		/// Define right group for manage Company Users.
		/// </summary>
		public const string CompanyUserManageRights = "CompanyUserManageRights";


		public static Dictionary<string, string[]> Roles = new Dictionary<string, string[]>
		{
			{ CompanyUserManageRights,  new []{ UserRoles.SystemAdmin, UserRoles.CompanyPowerUser }}
		};
	}
}

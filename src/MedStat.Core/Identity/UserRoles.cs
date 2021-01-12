
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Metadata;

namespace MedStat.Core.Identity
{
	public static class UserRoles
	{
		public const string SystemAdmin = "system_admin";

		// Company data management:
		public const string CompanyUserManager = "company_user_manager";
		public const string CompanyStaffManager = "company_staff_manager";


		internal static string[] GetAllRoles()
		{
			return 
				new[]
				{
					SystemAdmin,

					CompanyUserManager,
					CompanyStaffManager
				};
		}
	}


	public class AccessPolicies
	{
		public const string CompanyUserManageRights = "CompanyUserManageRights";


		public static Dictionary<string, string[]> Roles = new Dictionary<string, string[]>
		{
			{ CompanyUserManageRights,  new []{ UserRoles.SystemAdmin, UserRoles.CompanyUserManager }}
		};
	}
}

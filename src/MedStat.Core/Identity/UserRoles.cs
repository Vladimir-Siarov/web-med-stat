
namespace MedStat.Core.Identity
{
	public static class UserRoles
	{
		public const string SystemAdmin = "system_admin";

		// Company data management:
		public const string CompanyAccessManager = "company_access_manager";
		public const string CompanyStaffManager = "company_staff_manager";


		internal static string[] GetAllRoles()
		{
			return 
				new[]
				{
					SystemAdmin,

					CompanyAccessManager,
					CompanyStaffManager
				};
		}
	}
}

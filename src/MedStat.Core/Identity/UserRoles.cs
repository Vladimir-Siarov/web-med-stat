
namespace MedStat.Core.Identity
{
	public static class UserRoles
	{
		public const string SystemAdmin = "system_admin";


		internal static string[] GetAllRoles()
		{
			return 
				new[]
				{
					SystemAdmin
				};
		}
	}
}

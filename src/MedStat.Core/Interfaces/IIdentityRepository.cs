using System.Collections.Generic;
using System.Threading.Tasks;
using MedStat.Core.Identity;

namespace MedStat.Core.Interfaces
{
	public interface IIdentityRepository
	{
		/// <summary>
		/// Find SystemUser by specified phone number.
		/// </summary>
		/// <param name="phoneNumber"></param>
		/// <returns></returns>
		Task<SystemUser> FindByPhoneNumberAsync(string phoneNumber);
		
		/// <summary>
		/// Creates SystemUser with specified data and adds it to the specified roles.<br/>
		/// Note: Method doesn't rollback partial results if one or several sub-actions were failed!
		/// </summary>
		/// <param name="userData"></param>
		/// <param name="userRoles"></param>
		/// <returns></returns>
		Task<SystemUser> CreateSystemUserByPhoneNumberAsync_UnderOuterTransaction(SystemUser userData,
			IEnumerable<string> userRoles);


		/// <summary>
		/// Adds specified user to the specified roles.
		/// </summary>
		/// <param name="user"></param>
		/// <param name="roles"></param>
		/// <param name="checkBeforeAdding"></param>
		/// <returns></returns>
		Task<IEnumerable<string>> AddToRolesAsync(SystemUser user, IEnumerable<string> roles, bool checkBeforeAdding = false);

		/// <summary>
		/// Get Roles assigned to the specified user.
		/// </summary>
		/// <param name="user"></param>
		/// <returns></returns>
		Task<IEnumerable<string>> GetUserRolesAsync(SystemUser user);


		/// <summary>
		/// Change password for specified user.
		/// </summary>
		/// <param name="phoneNumber"></param>
		/// <param name="password"></param>
		/// <returns></returns>
		Task ChangeUserPasswordAsync(string phoneNumber, string password);

		/// <summary>
		/// Set "ChangePasswordRequired" flag for specified user.
		/// </summary>
		/// <param name="phoneNumber"></param>
		/// <param name="isChangePasswordRequired"></param>
		/// <returns></returns>
		Task SetPasswordChangeRequiredFlagAsync(string phoneNumber, bool isChangePasswordRequired);
	}
}

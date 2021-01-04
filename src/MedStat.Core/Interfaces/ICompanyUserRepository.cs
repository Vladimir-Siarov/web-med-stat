using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MedStat.Core.Identity;
using MedStat.Core.Info.Company;

namespace MedStat.Core.Interfaces
{
	public interface ICompanyUserRepository
	{
		/// <summary>
		/// Get CompanyUser info object for specified cmp. user.
		/// </summary>
		/// <param name="cmpUserId"></param>
		/// <returns></returns>
		Task<CompanyUserInfo> GetCompanyUserAsync(int cmpUserId);


		/// <summary>
		/// Creates CompanyUser instance in DB and assign it to the specified company.
		/// </summary>
		/// <param name="companyId"></param>
		/// <param name="description"></param>
		/// <param name="login"></param>
		/// <param name="canManageCompanyAccess"></param>
		/// <param name="canManageCompanyStaff"></param>
		/// <returns></returns>
		Task<int> CreateCompanyUserAsync(int companyId,
			// Cmp User data
			string description,
			// Login data
			SystemUser login,
			// user rights
			bool canManageCompanyAccess, bool canManageCompanyStaff);
	}
}

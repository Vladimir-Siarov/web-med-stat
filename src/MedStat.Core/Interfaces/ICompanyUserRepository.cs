using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MedStat.Core.BE.Company;
using MedStat.Core.Identity;
using MedStat.Core.Info;
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
		Task<CompanyUserInfo> GetCompanyUserInfoAsync(int cmpUserId);


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

		/// <summary>
		/// Update data of specified CompanyUser.
		/// </summary>
		/// <param name="cmpUserId"></param>
		/// <param name="description"></param>
		/// <param name="login"></param>
		/// <param name="canManageCompanyAccess"></param>
		/// <param name="canManageCompanyStaff"></param>
		/// <returns></returns>
		Task UpdateCompanyUserAsync(int cmpUserId,
			// Cmp User data
			string description,
			// Login data
			SystemUser login,
			// user rights
			bool canManageCompanyAccess, bool canManageCompanyStaff);

		/// <summary>
		/// Delete specified CompanyUser.
		/// </summary>
		/// <param name="cmpUserId"></param>
		/// <returns></returns>
		Task DeleteCompanyUserAsync(int cmpUserId);


		/// <summary>
		/// Finds company Users and sorts results by specified params.
		/// </summary>
		/// <param name="nameSearchTerms"></param>
		/// <param name="sortByProperty"></param>
		/// <param name="isSortByAsc"></param>
		/// <param name="skip"></param>
		/// <param name="take"></param>
		/// <returns></returns>
		Task<SearchResult<CompanyUser>> FindCompanyUsersAsync(string nameSearchTerms,
			string sortByProperty, bool isSortByAsc,
			int skip, int take);
	}
}

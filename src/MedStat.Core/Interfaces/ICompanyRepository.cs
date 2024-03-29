﻿using System.Threading.Tasks;
using MedStat.Core.BE.Company;
using MedStat.Core.Info;
using MedStat.Core.Info.Company;

namespace MedStat.Core.Interfaces
{
	public interface ICompanyRepository : ICompanyUserRepository
	{
		/// <summary>
		/// Returns main data for specified company.
		/// </summary>
		/// <param name="companyId"></param>
		/// <returns></returns>
		Task<Company> GetCompanyMainDataAsync(int companyId);

		/// <summary>
		/// Returns main and requisites data for specified company.
		/// </summary>
		/// <param name="companyId"></param>
		/// <returns></returns>
		Task<Company> GetCompanyWithRequisitesAsync(int companyId);


		/// <summary>
		/// Creates Company instance in DB.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="description"></param>
		/// <returns></returns>
		Task<int> CreateCompanyAsync(string name, string description);

		/// <summary>
		/// Remove specified company from DB.
		/// </summary>
		/// <param name="companyId"></param>
		/// <returns></returns>
		Task DeleteCompanyAsync(int companyId);


		/// <summary>
		/// Updates main data for specified company.
		/// </summary>
		/// <param name="companyId"></param>
		/// <param name="name"></param>
		/// <param name="description"></param>
		/// <returns></returns>
		Task UpdateCompanyMainDataAsync(int companyId, string name, string description);

		/// <summary>
		/// Updates requisites data for specified company.
		/// </summary>
		/// <param name="companyId"></param>
		/// <param name="mainReqData"></param>
		/// <param name="bankReqData"></param>
		/// <returns></returns>
		Task UpdateCompanyRequisitesAsync(int companyId,
			CompanyMainRequisites mainReqData, CompanyBankRequisites bankReqData);


		/// <summary>
		/// Finds companies and sorts results by specified params.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="sortByProperty"></param>
		/// <param name="isSortByAsc"></param>
		/// <param name="skip"></param>
		/// <param name="take"></param>
		/// <returns></returns>
		Task<SearchResult<CompanySearchInfo>> FindCompaniesAsync(string name,
			string sortByProperty, bool isSortByAsc,
			int skip, int take);
	}
}

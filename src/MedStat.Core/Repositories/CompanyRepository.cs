using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using MedStat.Core.BE.Company;
using MedStat.Core.DAL;
using MedStat.Core.Helpers;
using MedStat.Core.Info;
using MedStat.Core.Info.Company;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MedStat.Core.Repositories
{
	public class CompanyRepository : BaseRepository
	{
		public CompanyRepository(MedStatDbContext dbContext, ILogger<CompanyRepository> logger, string userUid)
			: base(dbContext, logger, userUid)
		{
		}

		#region Get

		public async Task<SearchResult<CompanySearchInfo>> FindCompaniesAsync(string name, 
			string sortByProperty, bool isSortByAsc,
			int skip, int take)
		{
			var q = this.DbContext.Companies.Select(c => c);

			if (!string.IsNullOrEmpty(name))
			{
				q = q.Where(c => c.Name.Contains(name));
			}

			// TODO: Add support for TrackedPersonCnt & AccountCnt properties

			var result = new SearchResult<CompanySearchInfo>();
			
			result.TotalRecords = await q.CountAsync();

			#region Sorting

			if (!string.IsNullOrEmpty(sortByProperty))
			{
				switch (sortByProperty)
				{
					case nameof(CompanySearchInfo.Id):
						q = isSortByAsc ? q.OrderBy(c => c.Id) : q.OrderByDescending(c => c.Id);
						break;

					case nameof(CompanySearchInfo.Name):
						q = isSortByAsc ? q.OrderBy(c => c.Name) : q.OrderByDescending(c => c.Name);
						break;

					case nameof(CompanySearchInfo.Description):
						q = isSortByAsc ? q.OrderBy(c => c.Description) : q.OrderByDescending(c => c.Description);
						break;

					case nameof(CompanySearchInfo.AccountCnt):
						// TODO
						break;

					case nameof(CompanySearchInfo.TrackedPersonCnt):
						// TODO
						break;

					default:
						throw new NotSupportedException(sortByProperty);
				}
			}

			#endregion
			
			var companies = await q.Skip(skip).Take(take).AsNoTracking().ToArrayAsync();

			result.Data = companies
				.Select(c => new CompanySearchInfo
				{
					Id = c.Id,
					Name = c.Name,
					Description = c.Description,
					AccountCnt = 0, // TODO
					TrackedPersonCnt = 0 // TODO
				})
				.ToArray();

			return result;
		}


		public async Task<Company> GetCompanyMainData(int companyId)
		{
			Company cmp = await this.DbContext.Companies
				.AsNoTracking()
				.FirstOrDefaultAsync(c => c.Id == companyId);

			return cmp;
		}

		public async Task<Company> GetCompanyRequisitesAsync(int companyId)
		{
			Company cmp = await this.DbContext.Companies
				.Include(c => c.Requisites)
				.AsNoTracking()
				.FirstOrDefaultAsync(c => c.Id == companyId);

			return cmp;
		}

		#endregion

		#region Create

		public async Task<int> CreateCompanyAsync(string name, string description)
		{
			if (name == null)
				throw new ArgumentNullException(nameof(name));

			// TODO: Check name for unique

			try
			{
				var createdDate = DateTime.UtcNow;
				var newCompany = new Company
				{
					Name = name,
					Description = description,

					CreatedUtc = createdDate,
					UpdatedUtc = createdDate,

					Requisites = new CompanyRequisites
					{
						MainRequisites = new CompanyMainRequisites
						{
							Name = name
						},
						BankRequisites = new CompanyBankRequisites(),
						UpdatedUtc = createdDate
					}
				};

				this.DbContext.Companies.Add(newCompany);
				await this.DbContext.SaveChangesAsync();

				this.Logger.LogInformation("Company \"{0}\" ({1}) was created successfully by {2}",
					newCompany.Name, newCompany.Id, this.UserUid);

				return newCompany.Id;
			}
			catch (Exception ex)
			{
				this.Logger.LogError(ex, "Company creation was failed");
				throw;
			}
		}

		#endregion

		#region Update

		public async Task UpdateCompanyMainDataAsync(int companyId, string name, string description)
		{
			if (name == null)
				throw new ArgumentNullException(nameof(name));

			// TODO: Check name for unique

			try
			{
				var dbCompany = await this.DbContext.Companies.FirstOrDefaultAsync(c => c.Id == companyId);
				if (dbCompany == null)
				{
					throw new OperationCanceledException(string.Format(
						this.MessagesManager.GetString("Company with ID = {0} is not found"),
						companyId));
				}

				dbCompany.Name = name;
				dbCompany.Description = description;
				dbCompany.UpdatedUtc = DateTime.UtcNow;

				await this.DbContext.SaveChangesAsync();

				this.Logger.LogInformation("Main data of Company \"{0}\" ({1}) was updated by {2}",
					dbCompany.Name, dbCompany.Id, this.UserUid);
			}
			catch (Exception ex)
			{
				this.Logger.LogError(ex, "Company Main data update action was failed");
				throw;
			}
		}

		public async Task UpdateCompanyRequisitesAsync(int companyId, 
			CompanyMainRequisites mainReqData, CompanyBankRequisites bankReqData)
		{
			if(mainReqData == null)
				throw new ArgumentNullException(nameof(mainReqData));

			if(string.IsNullOrEmpty(mainReqData.Name))
				throw new ArgumentNullException($"{nameof(mainReqData)}.{nameof(mainReqData.Name)}");
			
			// TODO: Check requisites for unique
			
			try
			{
				var dbCompany = await this.DbContext.Companies
					.Include(c => c.Requisites)
					.FirstOrDefaultAsync(c => c.Id == companyId);

				if(dbCompany == null)
				{
					throw new OperationCanceledException(string.Format(
						this.MessagesManager.GetString("Company with ID = {0} is not found"),
						companyId));
				}

				dbCompany.Requisites.MainRequisites = mainReqData.CreateCopy();
				if (bankReqData != null)
				{
					dbCompany.Requisites.BankRequisites = bankReqData.CreateCopy();
				}
				dbCompany.Requisites.UpdatedUtc = DateTime.UtcNow;

				await this.DbContext.SaveChangesAsync();

				this.Logger.LogInformation("Requisites of Company \"{0}\" ({1}) was updated by {2}",
					dbCompany.Name, dbCompany.Id, this.UserUid);
			}
			catch (Exception ex)
			{
				this.Logger.LogError(ex, "Company Requisites update action was failed");
				throw;
			}
		}

		#endregion
	}
}

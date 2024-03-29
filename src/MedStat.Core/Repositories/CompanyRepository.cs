﻿using System;
using System.Linq;
using System.Threading.Tasks;
using MedStat.Core.BE.Company;
using MedStat.Core.DAL;
using MedStat.Core.Helpers;
using MedStat.Core.Info;
using MedStat.Core.Info.Company;
using MedStat.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MedStat.Core.Repositories
{
	public partial class CompanyRepository : BaseRepository, ICompanyRepository
	{
		private readonly IIdentityRepository _identityRepository;


		public CompanyRepository(IIdentityRepository identityRepository,
			MedStatDbContext dbContext, ILogger<CompanyRepository> logger, string userUid)
			: base(dbContext, logger, userUid)
		{
			this._identityRepository = identityRepository;
		}


		#region Get
		
		public async Task<Company> GetCompanyMainDataAsync(int companyId)
		{
			Company cmp = await this.DbContext.Companies
				.AsNoTracking()
				.FirstOrDefaultAsync(c => c.Id == companyId);

			return cmp;
		}

		public async Task<Company> GetCompanyWithRequisitesAsync(int companyId)
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
			if (string.IsNullOrEmpty(name))
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

				this.Logger.LogInformation("Company {@Company} was created successfully by {UserUid}",
					new { newCompany.Id, newCompany.Name }, this.UserUid);

				return newCompany.Id;
			}
			catch (Exception ex)
			{
				this.Logger.LogError(ex, "Company creation was failed. {@params}", 
					new { name, description });
				throw;
			}
		}

		#endregion

		#region Update / Delete

		public async Task UpdateCompanyMainDataAsync(int companyId, string name, string description)
		{
			if (string.IsNullOrEmpty(name))
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

				this.Logger.LogInformation("Main data of Company {@Company} was updated by {UserUid}",
					new { dbCompany.Id, dbCompany.Name }, this.UserUid);
			}
			catch (Exception ex)
			{
				this.Logger.LogError(ex, "Company Main data update action was failed. {@params}",
					new { companyId, name, description });
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
				if (dbCompany.Requisites == null)
				{
					throw new OperationCanceledException(string.Format(
						this.MessagesManager.GetString("Requisites for Company with ID = {0} does not exist"),
						companyId));
				}

				dbCompany.Requisites.MainRequisites = mainReqData.CreateCopy();
				if (bankReqData != null)
				{
					dbCompany.Requisites.BankRequisites = bankReqData.CreateCopy();
				}
				dbCompany.Requisites.UpdatedUtc = DateTime.UtcNow;

				await this.DbContext.SaveChangesAsync();

				this.Logger.LogInformation("Requisites of Company {@Company} was updated by {UserUid}",
					new { dbCompany.Id, dbCompany.Name }, this.UserUid);
			}
			catch (Exception ex)
			{
				this.Logger.LogError(ex, "Company Requisites update action was failed. {@params}",
					new { companyId, mainReqData, bankReqData });
				throw;
			}
		}


		public async Task DeleteCompanyAsync(int companyId)
		{
			try
			{
				var dbCompany = await this.DbContext.Companies.FirstOrDefaultAsync(c => c.Id == companyId);
				if (dbCompany == null)
				{
					throw new OperationCanceledException(string.Format(
						this.MessagesManager.GetString("Company with ID = {0} is not found"),
						companyId));
				}

				// Check that company doesn't have sub-data (except Requisites)
				{
					bool isCmpUserExist = this.DbContext.CompanyUsers.Any(cu => cu.CompanyId == companyId);
					if(isCmpUserExist)
					{ 
						throw new OperationCanceledException(string.Format(
							this.MessagesManager.GetString("Company with ID = {0} contains users and cannot be deleted"),
							companyId));
					}

					// TODO: Staff, Devices and etc ...
				}

				await using (var transaction = await this.DbContext.Database.BeginTransactionAsync())
				{
					var requisites = await this.DbContext.CompanyRequisites
						.FirstOrDefaultAsync(r => r.CompanyId == dbCompany.Id);

					if (requisites != null)
					{
						this.DbContext.CompanyRequisites.Remove(requisites);
						await this.DbContext.SaveChangesAsync();
					}

					var cmpInfo = new {dbCompany.Id, dbCompany.Name};

					this.DbContext.Companies.Remove(dbCompany);
					await this.DbContext.SaveChangesAsync();

					await transaction.CommitAsync();

					this.Logger.LogInformation("Company {@Company} was deleted by {UserUid}",
						cmpInfo, this.UserUid);
				}
			}
			catch (Exception ex)
			{
				this.Logger.LogError(ex, "Company delete action was failed. {@params}", 
					new { companyId });
				throw;
			}
		}

		#endregion


		#region Search

		public async Task<SearchResult<CompanySearchInfo>> FindCompaniesAsync(string name, 
			string sortByProperty, bool isSortByAsc,
			int skip, int take)
		{
			var q = this.DbContext.Companies.Select(c => c);

			if (!string.IsNullOrEmpty(name))
			{
				q = q.Where(c => c.Name.Contains(name));
			}

			// TODO: Add support for TrackedPersonCnt & UserCnt properties

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

					case nameof(CompanySearchInfo.UserCnt):
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
					UserCnt = 0, // TODO
					TrackedPersonCnt = 0 // TODO
				})
				.ToArray();

			return result;
		}

		#endregion
	}
}

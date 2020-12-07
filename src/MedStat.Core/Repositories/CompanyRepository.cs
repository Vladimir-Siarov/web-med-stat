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

		public async Task<IEnumerable<Company>> GetCompaniesAsync(string name)
		{
			var q = this.DbContext.Companies.Select(c => c);

			if (!string.IsNullOrEmpty(name))
			{
				q = q.Where(c => c.Name.Contains(name));
			}

			var companies = await q.AsNoTracking().ToArrayAsync();

			return companies;
		}


		public async Task<Company> GetCompanyMainData(int companyId)
		{
			var cmpData = await this.DbContext.Companies
				.Where(c => c.Id == companyId)
				.Select(c => new { name = c.Name, desc = c.Description  })
				.FirstOrDefaultAsync();

			return 
				cmpData != null 
					? new Company { Id = companyId, Name = cmpData.name, Description = cmpData.desc } 
					: null;
		}

		public async Task<Company> GetCompanyRequisitesAsync(int companyId)
		{
			Company cmp = await this.DbContext.Companies.FirstOrDefaultAsync(c => c.Id == companyId);

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
				var newCompany = new Company
				{
					Name = name,
					Description = description,

					MainRequisites = new CompanyMainRequisites
					{
						Name = name
					},
					BankRequisites = new CompanyBankRequisites()
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
				var dbCompany = await this.DbContext.Companies.FirstOrDefaultAsync(c => c.Id == companyId);
				if(dbCompany == null)
				{
					throw new OperationCanceledException(string.Format(
						this.MessagesManager.GetString("Company with ID = {0} is not found"),
						companyId));
				}

				dbCompany.MainRequisites = mainReqData.CreateCopy();
				if (bankReqData != null)
				{
					dbCompany.BankRequisites = bankReqData.CreateCopy();
				}

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

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MedStat.Core.BE.Company;
using MedStat.Core.DAL;
using MedStat.Core.Helpers;
using Microsoft.Extensions.Logging;

namespace MedStat.Core.Repositories
{
	public class CompanyRepository : BaseRepository
	{
		public CompanyRepository(MedStatDbContext dbContext, ILogger<CompanyRepository> logger, string userUid)
			: base(dbContext, logger, userUid)
		{
		}


		public async Task<int> CreateCompanyAsync(CompanyMainRequisites mainReqData, CompanyBankRequisites bankReqData)
		{
			if(mainReqData == null)
				throw new ArgumentNullException(nameof(mainReqData));

			if(string.IsNullOrEmpty(mainReqData.Name))
				throw new ArgumentNullException($"{nameof(mainReqData)}.{nameof(mainReqData.Name)}");

			
			// TODO: Check requisites for unique

			try
			{
				var newCompany = new Company();

				newCompany.MainRequisites = mainReqData.CreateCopy();
				newCompany.BankRequisites = bankReqData != null 
					? bankReqData.CreateCopy() 
					: new CompanyBankRequisites();

				this.DbContext.Companies.Add(newCompany);
				await this.DbContext.SaveChangesAsync();

				this.Logger.LogInformation("Company \"{0}\" ({1}) was created successfully by {2}",
					newCompany.MainRequisites.Name, newCompany.Id, this.UserUid);

				return newCompany.Id;
			}
			catch (Exception ex)
			{
				this.Logger.LogError(ex, "Company creation was failed");
			}

			return -1;
		}
	}
}

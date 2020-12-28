using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MedStat.Core.BE.Company;
using MedStat.Core.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MedStat.Core.Repositories
{
	public partial class CompanyRepository
	{
		#region Create

		public async Task<int> CreateCompanyUserAsync(int companyId,
			// Cmp User data
			string description,
			// Login data
			SystemUser login,
			// user rights
			bool canManageCompanyAccess, bool canManageCompanyStaff)
		{
			if (login == null)
				throw new ArgumentNullException(nameof(login));

			bool isCompanyExist = await this.DbContext.Companies.AnyAsync(c => c.Id == companyId);
			if (!isCompanyExist)
			{
				throw new OperationCanceledException(string.Format(
					this.MessagesManager.GetString("Company with ID = {0} is not found"),
					companyId));
			}

			try
			{
				var userRoles = new List<string>();
				{
					if (canManageCompanyAccess)
						userRoles.Add(UserRoles.CompanyAccessManager);
					
					if (canManageCompanyStaff)
						userRoles.Add(UserRoles.CompanyStaffManager);
				}

				var newCmpUser = new CompanyUser
				{
					Description = description,
					CompanyId = companyId
				};

				await using (var transaction = await this.DbContext.Database.BeginTransactionAsync())
				{
					var newLogin = await _identityRepository
						.CreateSystemUserByPhoneNumberAsync_UnderOuterTransaction(login, userRoles);

					newCmpUser.Login = newLogin;

					this.DbContext.CompanyUsers.Add(newCmpUser);
					await this.DbContext.SaveChangesAsync();

					await transaction.CommitAsync();
				}

				this.Logger.LogInformation("CompanyUser {@User} was created successfully",
					new { newCmpUser.Id, newCmpUser.Login.UserName });

				return newCmpUser.Id;
			}
			catch (Exception ex)
			{
				this.Logger.LogError(ex, "CompanyUser creation was failed");
				throw;
			}
		}

		#endregion
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MedStat.Core.BE.Company;
using MedStat.Core.Identity;
using MedStat.Core.Info;
using MedStat.Core.Info.Company;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MedStat.Core.Repositories
{
	public partial class CompanyRepository
	{
		#region Get

		public async Task<CompanyUserInfo> GetCompanyUserInfoAsync(int cmpUserId)
		{
			CompanyUserInfo userInfo = null;

			CompanyUser cmpUser = await this.DbContext.CompanyUsers
				.Include(cu => cu.Login)
				.AsNoTracking()
				.FirstOrDefaultAsync(cu => cu.Id == cmpUserId);

			if (cmpUser != null)
			{
				userInfo = new CompanyUserInfo
				{
					User = cmpUser
				};

				var roles = await this._identityRepository.GetUserRolesAsync(cmpUser.Login);

				userInfo.IsPowerUser = roles != null && roles.Contains(UserRoles.CompanyPowerUser);
			}

			return userInfo;
		}

		#endregion

		#region Create / Update / Delete

		public async Task<int> CreateCompanyUserAsync(int companyId,
			// Cmp User data
			string description,
			// Login data
			SystemUser login,
			// user rights
			bool isPowerUser)
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
				var userRoles = FormatCompanyUserRoles(isPowerUser);

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

		public async Task UpdateCompanyUserAsync(int cmpUserId,
			// Cmp User data
			string description,
			// Login data
			SystemUser login,
			// user rights
			bool isPowerUser)
		{
			#region Validation

			if (login != null)
			{
				if (string.IsNullOrEmpty(login.FirstName))
					throw new ArgumentNullException($"{nameof(login)}.{nameof(login.FirstName)}");
				if (string.IsNullOrEmpty(login.Surname))
					throw new ArgumentNullException($"{nameof(login)}.{nameof(login.Surname)}");
			}

			#endregion

			try
			{
				var dbCmpUser = await this.DbContext.CompanyUsers
					.Include(cu => cu.Login)
					.FirstOrDefaultAsync(c => c.Id == cmpUserId);

				if (dbCmpUser == null)
				{
					throw new OperationCanceledException(string.Format(
						this.MessagesManager.GetString("Company user with ID = {0} is not found"),
						cmpUserId));
				}

				await using (var transaction = await this.DbContext.Database.BeginTransactionAsync())
				{
					dbCmpUser.Description = description;

					if (login != null)
					{
						dbCmpUser.Login.FirstName = login.FirstName;
						dbCmpUser.Login.Surname = login.Surname;
						dbCmpUser.Login.Patronymic = login.Patronymic;
					}

					await this.DbContext.SaveChangesAsync();

					#region Update Roles

					await _identityRepository.RemoveFromRolesAsync(dbCmpUser.Login, GetAllCompanyUserRoles(), true);

					var userRoles = FormatCompanyUserRoles(isPowerUser);
					await _identityRepository.AddToRolesAsync(dbCmpUser.Login, userRoles, true);
					
					#endregion

					await transaction.CommitAsync();
				}
				

				this.Logger.LogInformation("CompanyUser {@User} was updated by {UserUid}",
					new { dbCmpUser.Id, dbCmpUser.Login.UserName }, this.UserUid);
			}
			catch (Exception ex)
			{
				this.Logger.LogError(ex, "CompanyUser update action was failed");
				throw;
			}
		}

		public async Task DeleteCompanyUserAsync(int cmpUserId)
		{
			try
			{
				var dbCmpUser = await this.DbContext.CompanyUsers
					.Include(c => c.Login)
					.FirstOrDefaultAsync(c => c.Id == cmpUserId);

				if (dbCmpUser == null)
				{
					throw new OperationCanceledException(string.Format(
						this.MessagesManager.GetString("Company user with ID = {0} is not found"),
						cmpUserId));
				}
				
				await using (var transaction = await this.DbContext.Database.BeginTransactionAsync())
				{
					var cmpUserInfo = new {dbCmpUser.Id, dbCmpUser.SystemUserId, dbCmpUser.Login.UserName};
					int systemUserId = dbCmpUser.SystemUserId;

					this.DbContext.CompanyUsers.Remove(dbCmpUser);
					await this.DbContext.SaveChangesAsync();

					await this._identityRepository.DeleteSystemUserAsync_UnderOuterTransaction(systemUserId);

					await transaction.CommitAsync();

					this.Logger.LogInformation("CompanyUser {@cmpUser} was deleted by {UserUid}",
						cmpUserInfo, this.UserUid);
				}
			}
			catch (Exception ex)
			{
				this.Logger.LogError(ex, "CompanyUser delete action was failed");
				throw;
			}
		}

		#endregion

		#region Search

		public async Task<SearchResult<CompanyUser>> FindCompanyUsersAsync(string nameSearchTerms,
			string sortByProperty, bool isSortByAsc,
			int skip, int take)
		{
			var q = this.DbContext.CompanyUsers
				.Include(cu => cu.Login)
				.Select(cu => cu);

			if (!string.IsNullOrEmpty(nameSearchTerms))
			{
				// Searching by each word in name search terms
				foreach (string searchTerm in nameSearchTerms.Split(' ', StringSplitOptions.RemoveEmptyEntries))
				{
					q = q.Where(cu => cu.Login.FirstName.Contains(searchTerm)
														|| cu.Login.Surname.Contains(searchTerm)
														|| cu.Login.Patronymic.Contains(searchTerm));
				}
			}

			var result = new SearchResult<CompanyUser>();

			result.TotalRecords = await q.CountAsync();

			#region Sorting

			if (!string.IsNullOrEmpty(sortByProperty))
			{
				switch (sortByProperty)
				{
					case nameof(CompanyUser.Id):
						q = isSortByAsc ? q.OrderBy(c => c.Id) : q.OrderByDescending(c => c.Id);
						break;

					case nameof(CompanyUser.Login.FirstName):
						q = isSortByAsc ? q.OrderBy(c => c.Login.FirstName) : q.OrderByDescending(c => c.Login.FirstName);
						break;
					case nameof(CompanyUser.Login.Surname):
						q = isSortByAsc ? q.OrderBy(c => c.Login.Surname) : q.OrderByDescending(c => c.Login.Surname);
						break;
					case nameof(CompanyUser.Login.Patronymic):
						q = isSortByAsc ? q.OrderBy(c => c.Login.Patronymic) : q.OrderByDescending(c => c.Login.Patronymic);
						break;

					case nameof(CompanyUser.Login.PhoneNumber):
						q = isSortByAsc ? q.OrderBy(c => c.Login.PhoneNumber) : q.OrderByDescending(c => c.Login.PhoneNumber);
						break;

					case nameof(CompanyUser.Description):
						q = isSortByAsc ? q.OrderBy(c => c.Description) : q.OrderByDescending(c => c.Description);
						break;

					default:
						throw new NotSupportedException(sortByProperty);
				}
			}

			#endregion

			result.Data = await q.Skip(skip).Take(take).AsNoTracking().ToArrayAsync();

			return result;
		}

		#endregion


		private static List<string> FormatCompanyUserRoles(bool isPowerUser)
		{
			var userRoles = new List<string>
			{
				isPowerUser ? UserRoles.CompanyPowerUser : UserRoles.CompanyUser
			};

			return userRoles;
		}

		private static string[] GetAllCompanyUserRoles()
		{
			var userRoles = new string[]
			{
				UserRoles.CompanyPowerUser,
				UserRoles.CompanyUser
			};

			return userRoles;
		}
	}
}

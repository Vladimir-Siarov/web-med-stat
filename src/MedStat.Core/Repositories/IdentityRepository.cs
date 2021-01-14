using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MedStat.Core.DAL;
using MedStat.Core.Identity;
using MedStat.Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MedStat.Core.Repositories
{
	public class IdentityRepository : BaseRepository, IIdentityRepository
	{
		private readonly UserManager<SystemUser> _userManager;


		public IdentityRepository(UserManager<SystemUser> userManager,
			MedStatDbContext dbContext, ILogger<IdentityRepository> logger, string userUid)
			: base(dbContext, logger, userUid)
		{
			this._userManager = userManager;
		}


		// SystemUser:

		public async Task<SystemUser> FindByPhoneNumberAsync(string phoneNumber)
		{
			if (string.IsNullOrEmpty(phoneNumber))
				return null;

			var normalizedPhoneNumber = NormalizePhoneNumber(phoneNumber);
			var user = await this.DbContext.SystemUsers
				.FirstOrDefaultAsync(su => su.NormalizedPhoneNumber == normalizedPhoneNumber);

			return user;
		}
		
		public async Task<SystemUser> CreateSystemUserByPhoneNumberAsync_UnderOuterTransaction(SystemUser userData, 
			IEnumerable<string> userRoles)
		{
			#region Validation

			if (userData == null)
				throw new ArgumentNullException(nameof(userData));
			
			if (string.IsNullOrEmpty(userData.FirstName))
				throw new ArgumentNullException($"{nameof(userData)}.{nameof(userData.FirstName)}");
			if (string.IsNullOrEmpty(userData.Surname))
				throw new ArgumentNullException($"{nameof(userData)}.{nameof(userData.Surname)}");
			
			if (string.IsNullOrEmpty(userData.PhoneNumber))
				throw new ArgumentNullException($"{nameof(userData)}.{nameof(userData.PhoneNumber)}");
			
			await CheckForUniquePhoneNumberAsync(userData.PhoneNumber);

			#endregion

			string normalizedPhoneNumber = NormalizePhoneNumber(userData.PhoneNumber);
			var user = new SystemUser
			{
				UserName = normalizedPhoneNumber,
				IsPasswordChangeRequired = true,

				//Email = userData.Email,
				PhoneNumber = userData.PhoneNumber,
				NormalizedPhoneNumber = normalizedPhoneNumber,

				FirstName = userData.FirstName,
				Surname = userData.Surname,
				Patronymic = userData.Patronymic
			};

			var result = await _userManager.CreateAsync(user);
			if (!result.Succeeded)
			{ 
				throw GenerateIdentityResultException(result,
					this.MessagesManager.GetString("System User creation was failed"));
			}

			if (userRoles != null && userRoles.Any())
			{
				await this.AddToRolesAsync(user, userRoles);
			}

			return user;
		}

		public async Task DeleteSystemUserAsync_UnderOuterTransaction(int systemUserId)
		{
			var user = await this.DbContext.SystemUsers.FirstOrDefaultAsync(su => su.Id == systemUserId);
			if (user == null)
			{
				throw new OperationCanceledException(string.Format(
					this.MessagesManager.GetString("System user with Id = {0} is not found"),
					systemUserId));
			}

			var result = await _userManager.DeleteAsync(user);
			if (!result.Succeeded)
			{
				throw GenerateIdentityResultException(result,
					this.MessagesManager.GetString("System user delete action was failed"));
			}
		}

		protected async Task<SystemUser> GetSystemUserOrThrowExceptionAsync(string phoneNumber)
		{
			var normalizedPhoneNumber = NormalizePhoneNumber(phoneNumber);
			var user = await this.DbContext.SystemUsers
				.FirstOrDefaultAsync(su => su.NormalizedPhoneNumber == normalizedPhoneNumber);

			if (user == null)
			{
				throw new OperationCanceledException(string.Format(
					this.MessagesManager.GetString("User with phone number {0} is not found"),
					phoneNumber));
			}

			return user;
		}

		
		// Roles:

		public async Task<IEnumerable<string>> AddToRolesAsync(SystemUser user, IEnumerable<string> roles,
			bool checkBeforeAdding = false)
		{
			if(user == null)
				throw new ArgumentNullException(nameof(user));
			if (roles == null)
				throw new ArgumentNullException(nameof(roles));

			if (checkBeforeAdding)
			{
				List<string> addedRoles = new List<string>();

				foreach (string role in roles)
				{
					bool isInRole = await _userManager.IsInRoleAsync(user, role);
					if (!isInRole)
					{
						var result = await _userManager.AddToRoleAsync(user, role);
						if (!result.Succeeded)
						{
							throw GenerateIdentityResultException(result,
								this.MessagesManager.GetString("Adding user to roles was failed"));
						}

						addedRoles.Add(role);
					}
				}

				return addedRoles;
			}
			else
			{
				var result = await _userManager.AddToRolesAsync(user, roles);
				if (!result.Succeeded)
				{
					throw GenerateIdentityResultException(result,
						this.MessagesManager.GetString("Adding user to roles was failed"));
				}

				return roles;
			}
		}

		public async Task<IEnumerable<string>> RemoveFromRolesAsync(SystemUser user, IEnumerable<string> roles,
			bool checkBeforeAdding = false)
		{
			if (user == null)
				throw new ArgumentNullException(nameof(user));
			if (roles == null)
				throw new ArgumentNullException(nameof(roles));

			if (checkBeforeAdding)
			{
				List<string> removedRoles = new List<string>();

				foreach (string role in roles)
				{
					bool isInRole = await _userManager.IsInRoleAsync(user, role);
					if (isInRole)
					{
						var result = await _userManager.RemoveFromRoleAsync(user, role);
						if (!result.Succeeded)
						{
							throw GenerateIdentityResultException(result,
								this.MessagesManager.GetString("Removing user from roles was failed"));
						}

						removedRoles.Add(role);
					}
				}

				return removedRoles;
			}
			else
			{
				var result = await _userManager.RemoveFromRolesAsync(user, roles);
				if (!result.Succeeded)
				{
					throw GenerateIdentityResultException(result,
						this.MessagesManager.GetString("Removing user from roles was failed"));
				}

				return roles;
			}
		}

		public async Task<IEnumerable<string>> GetUserRolesAsync(SystemUser user)
		{
			if (user == null)
				throw new ArgumentNullException(nameof(user));

			return await _userManager.GetRolesAsync(user);
		}


		// Password:

		public async Task ChangeUserPasswordAsync(string phoneNumber, string password)
		{
			if (string.IsNullOrEmpty(phoneNumber))
				throw new ArgumentNullException(nameof(phoneNumber));
			if (string.IsNullOrEmpty(password))
				throw new ArgumentNullException(nameof(password));

			try
			{
				var user = await this.GetSystemUserOrThrowExceptionAsync(phoneNumber);
			
				if (await _userManager.HasPasswordAsync(user))
				{
					var token = await _userManager.GeneratePasswordResetTokenAsync(user);
					var result = await _userManager.ResetPasswordAsync(user, token, password);
					if (!result.Succeeded)
					{
						throw GenerateIdentityResultException(result,
							this.MessagesManager.GetString("Password reset operation was failed"));
					}
				}
				else
				{
					var result = await _userManager.AddPasswordAsync(user, password);
					if (!result.Succeeded)
					{
						throw GenerateIdentityResultException(result, 
							this.MessagesManager.GetString("Password add operation was failed"));
					}
				}

				this.Logger.LogInformation("Password for SystemUser {@User} was changed by {UserUid}",
					new { user.Id, user.UserName }, this.UserUid);
			}
			catch (Exception ex)
			{
				this.Logger.LogError(ex, "SystemUser change password action was failed. {@params}",
					new { phoneNumber });
				throw;
			}
		}

		public async Task SetPasswordChangeRequiredFlagAsync(string phoneNumber, bool isChangePasswordRequired)
		{
			if (string.IsNullOrEmpty(phoneNumber))
				throw new ArgumentNullException(nameof(phoneNumber));
			
			try
			{
				var user = await this.GetSystemUserOrThrowExceptionAsync(phoneNumber);

				user.IsPasswordChangeRequired = isChangePasswordRequired;
				await this.DbContext.SaveChangesAsync();

				this.Logger.LogInformation("\"PasswordChangeRequired\" flag was updated for SystemUser {@User} by {UserUid}",
					new { user.Id, user.UserName }, this.UserUid);
			}
			catch (Exception ex)
			{
				this.Logger.LogError(ex, "SystemUser update action for \"PasswordChangeRequired\" flag was failed. {@params}",
					new { phoneNumber, isChangePasswordRequired });
				throw;
			}
		}


		// Phone Number:

		public async Task ChangeUserPhoneNumberAsync(string phoneNumber, string newPhoneNumber)
		{
			if (string.IsNullOrEmpty(phoneNumber))
				throw new ArgumentNullException(nameof(phoneNumber));
			if (string.IsNullOrEmpty(newPhoneNumber))
				throw new ArgumentNullException(nameof(newPhoneNumber));

			try
			{
				await this.CheckForUniquePhoneNumberAsync(newPhoneNumber);

				var user = await this.GetSystemUserOrThrowExceptionAsync(phoneNumber);
				var oldUserName = user.UserName;

				string normalizedNewPhoneNumber = NormalizePhoneNumber(newPhoneNumber);

				await using (var transaction = await this.DbContext.Database.BeginTransactionAsync())
				{
					var result = await _userManager.SetPhoneNumberAsync(user, newPhoneNumber);
					if (!result.Succeeded)
					{
						throw GenerateIdentityResultException(result,
							this.MessagesManager.GetString("Phone number change operation was failed"));
					}

					result = await _userManager.RemovePasswordAsync(user);
					if (!result.Succeeded)
					{
						throw GenerateIdentityResultException(result,
							this.MessagesManager.GetString("Password reset operation was failed"));
					}

					result = await _userManager.SetUserNameAsync(user, normalizedNewPhoneNumber);
					if (!result.Succeeded)
					{
						throw GenerateIdentityResultException(result,
							this.MessagesManager.GetString("System User name change operation was failed"));
					}

					// reload user data
					user = this.DbContext.SystemUsers.First(u => u.Id == user.Id);

					user.NormalizedPhoneNumber = normalizedNewPhoneNumber;
					user.IsPasswordChangeRequired = true;

					await this.DbContext.SaveChangesAsync();

					await transaction.CommitAsync();
				}
				
				this.Logger.LogInformation("Phone number for SystemUser {@User} was changed to {NewNormalizedPhoneNumber} by {UserUid}",
					new { user.Id, UserName = oldUserName }, normalizedNewPhoneNumber, this.UserUid);
			}
			catch (Exception ex)
			{
				this.Logger.LogError(ex, "SystemUser change phone number action was failed. {@params}",
					new { phoneNumber, newPhoneNumber });
				throw;
			}
		}

		protected async Task CheckForUniquePhoneNumberAsync(string phoneNumber)
		{
			string normalizedPhoneNumber = NormalizePhoneNumber(phoneNumber);

			bool isPhoneNumberExist = await this.DbContext.SystemUsers
				.AnyAsync(su => su.NormalizedPhoneNumber == normalizedPhoneNumber);

			if (isPhoneNumberExist)
			{
				throw new OperationCanceledException(string.Format(
					this.MessagesManager.GetString("Phone Number {0} is already registered in the system"),
					normalizedPhoneNumber));
			}
		}


		private static string NormalizePhoneNumber(string number)
		{
			number = Regex.Replace(number, @"[^\d|\+]", "");

			// TODO: It's valid for Russia only! 
			// 89112299153 -> +79112299153
			if (number.Length == 11 && number.StartsWith('8'))
			{
				number = "+7" + number.Substring(1);
			}

			return number;
		}

		private static Exception GenerateIdentityResultException(IdentityResult result, string mainExMessage)
		{
			var innerException = result?.Errors?.FirstOrDefault() != null
				? new Exception(result.Errors.FirstOrDefault().Description)
				: null;

			return new Exception(mainExMessage, innerException);
		}
	}
}

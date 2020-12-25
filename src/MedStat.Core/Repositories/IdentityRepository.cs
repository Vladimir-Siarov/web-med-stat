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
			MedStatDbContext dbContext, ILogger<IdentityRepository> logger /*, string userUid*/)
			: base(dbContext, logger, null/*userUid*/)
		{
			this._userManager = userManager;
		}


		public async Task<SystemUser> FindByPhoneNumberAsync(string phoneNumber)
		{
			if (string.IsNullOrEmpty(phoneNumber))
				return null;

			var normalizedPhoneNumber = normalizePhoneNumber(phoneNumber);
			var user = await this.DbContext.SystemUsers
				.FirstOrDefaultAsync(su => su.NormalizedPhoneNumber == normalizedPhoneNumber);

			return user;
		}


		public async Task<SystemUser> CreateSystemUserByPhoneNumberAsync_UnderOuterTransaction(SystemUser userData, 
			string userPassword, IEnumerable<string> userRoles)
		{
			#region Validation

			if (userData == null)
				throw new ArgumentNullException(nameof(userData));
			if (string.IsNullOrEmpty(userPassword))
				throw new ArgumentNullException(nameof(userPassword));

			if (string.IsNullOrEmpty(userData.FirstName))
				throw new ArgumentNullException($"{nameof(userData)}.{nameof(userData.FirstName)}");
			if (string.IsNullOrEmpty(userData.Surname))
				throw new ArgumentNullException($"{nameof(userData)}.{nameof(userData.Surname)}");
			
			if (string.IsNullOrEmpty(userData.PhoneNumber))
				throw new ArgumentNullException($"{nameof(userData)}.{nameof(userData.PhoneNumber)}");
			
			string normalizedPhoneNumber = normalizePhoneNumber(userData.PhoneNumber);

			// Check for unique Phone Number
			{
				bool isPhoneNumberExist = await this.DbContext.SystemUsers
					.AnyAsync(su => su.NormalizedPhoneNumber == normalizedPhoneNumber);
				
				if (isPhoneNumberExist)
				{
					throw new OperationCanceledException(string.Format(
						this.MessagesManager.GetString("Phone Number {0} is already registered in the system"),
						normalizedPhoneNumber));
				}
			}

			#endregion

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

			var result = await _userManager.CreateAsync(user, userPassword);
			if (!result.Succeeded)
				throw new Exception(this.MessagesManager.GetString("System User creation was failed"));

			if (userRoles != null && userRoles.Any())
			{
				await this.AddToRolesAsync(user, userRoles);
			}

			return user;
		}

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
							throw new Exception(this.MessagesManager.GetString("Adding user to roles was failed"));

						addedRoles.Add(role);
					}
				}

				return addedRoles;
			}
			else
			{
				var result = await _userManager.AddToRolesAsync(user, roles);
				if (!result.Succeeded)
					throw new Exception(this.MessagesManager.GetString("Adding user to roles was failed"));

				return roles;
			}
		}


		private static string normalizePhoneNumber(string number)
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
	}
}

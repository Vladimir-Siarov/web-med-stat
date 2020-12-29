using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Localization;

using MedStat.Core.Identity;
using MedStat.Core.Interfaces;
using MedStat.Core.Resources;
using MedStat.WebAdmin.Classes.SharedResources;
using MedStat.WebAdmin.Models;

namespace MedStat.WebAdmin.Pages.Account
{
	public class ChangePasswordPageModel : SignInBasePageModel
	{
		private readonly IIdentityRepository _identityRepository;
		

		[BindProperty(SupportsGet = true)]
		[Required(ErrorMessage = Localizer.DataAnnotations.RequiredErrorMessage)]
		[Phone(ErrorMessage = Localizer.DataAnnotations.InvalidPhoneNumber)]
		[StringLength(20)]
		[Display(Name = "Mobile phone number")] // "Номер мобильного телефона"
		public string PhoneNumber { get; set; }

		[BindProperty]
		public bool IsNewPassword { get; set; } // password creation flag


		[BindProperty]
		[Required(ErrorMessage = Localizer.DataAnnotations.RequiredErrorMessage)]
		[Display(Name = "Confirmation code")] // "Код подтверждения"
		public string ConfirmationCode { get; set; }

		[BindProperty]
		[Required(ErrorMessage = Localizer.DataAnnotations.RequiredErrorMessage)]
		[DataType(DataType.Password)]
		[Display(Name = "New password")] // "Новый пароль"
		public string Password { get; set; }

		[BindProperty]
		[Required(ErrorMessage = Localizer.DataAnnotations.RequiredErrorMessage)]
		[DataType(DataType.Password)]
		[Compare("Password", ErrorMessage = "Field values \"{0}\" and \"{1}\" do not match.")]
		[Display(Name = "Confirm password")] // "Подтверждение пароля"
		public string ConfirmPassword { get; set; }


		// Wizard properties:

		public bool DisplayConfirmPhoneNumberSection { get; set; }


		public ChangePasswordPageModel(IIdentityRepository identityRepository, 
			SignInManager<SystemUser> signInManager,
			IStringLocalizer<IdentityResource> identityLocalizer,
			ILogger<ChangePasswordPageModel> logger)
			:base(logger, identityLocalizer, signInManager)
		{
			this._identityRepository = identityRepository;
		}


		public async Task<IActionResult> OnGetAsync()
		{
			DisplayConfirmPhoneNumberSection = true;

			SystemUser user = !string.IsNullOrEmpty(this.PhoneNumber)
				? await _identityRepository.FindByPhoneNumberAsync(this.PhoneNumber)
				: null;

			if (user != null)
			{
				this.IsNewPassword = string.IsNullOrEmpty(user.PasswordHash);

				// TODO: Sent confirmation code

				ViewData["info_message"] = string.Format(
					this.IdentityLocalizer["Confirmation code was sent to the following number: {0}"].Value,
					this.PhoneNumber);
			}
			else
			{
				ViewData["error_message"] = this.IdentityLocalizer["Invalid phone number"].Value;
			}

			return Page();
		}


		public async Task<IActionResult> OnPostCheckConfirmationCodeAsync()
		{
			DisplayConfirmPhoneNumberSection = true;

			ModelState[nameof(this.Password)].Errors.Clear();
			ModelState[nameof(this.Password)].ValidationState = ModelValidationState.Skipped;
			ModelState[nameof(this.ConfirmPassword)].Errors.Clear();
			ModelState[nameof(this.ConfirmPassword)].ValidationState = ModelValidationState.Skipped;

			if (ModelState.IsValid)
			{
				try
				{
					var user = await _identityRepository.FindByPhoneNumberAsync(this.PhoneNumber);
					if (user != null)
					{
						if (this.CheckConfirmationCode())
						{
							DisplayConfirmPhoneNumberSection = false;
						}
					}
					else
					{
						ViewData["error_message"] = this.IdentityLocalizer["Invalid phone number"].Value;
					}
				}
				catch (Exception ex)
				{
					ViewData["error_message"]
						= string.Format(this.IdentityLocalizer["Error has occurred: {0}"].Value, ex.Message);
				}
			}

			return Page();
		}

		public async Task<IActionResult> OnPostCheckPasswordAsync()
		{
			DisplayConfirmPhoneNumberSection = false;

			if (ModelState.IsValid)
			{
				try
				{
					var user = await _identityRepository.FindByPhoneNumberAsync(this.PhoneNumber);
					if (user != null)
					{
						if (this.CheckConfirmationCode())
						{
							await _identityRepository.ChangeUserPasswordAsync(this.PhoneNumber, this.Password);
							await _identityRepository.SetPasswordChangeRequiredFlagAsync(this.PhoneNumber, false);

							// Auto login
							if(!User.Identity.IsAuthenticated)
							{
								var actionResult = await this.SignInAsync(user.UserName, this.Password, false);
								if (actionResult != null)
									return actionResult;
							}
						}
					}
					else
					{
						ViewData["error_message"] = this.IdentityLocalizer["Invalid phone number"].Value;
					}
				}
				catch (Exception ex)
				{
					ViewData["error_message"]
						= string.Format(this.IdentityLocalizer["Error has occurred: {0}"].Value, ex.Message);
				}
			}

			return Page();
		}


		private bool CheckConfirmationCode()
		{
			bool rezult = false;

			// TODO: Check confirmation code
			rezult = true;
			
			if (!rezult)
			{
				ViewData["error_message"] = this.IdentityLocalizer["Invalid confirmation code"].Value;
			}

			return rezult;
		}
	}
}

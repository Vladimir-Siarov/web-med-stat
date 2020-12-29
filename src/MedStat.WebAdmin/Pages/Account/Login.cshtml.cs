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
	public class LoginPageModel : SignInBasePageModel
	{
		private readonly IIdentityRepository _identityRepository;
		

		[BindProperty]
		[Required(ErrorMessage = Localizer.DataAnnotations.RequiredErrorMessage)]
		[Phone(ErrorMessage = Localizer.DataAnnotations.InvalidPhoneNumber)]
		[StringLength(20)]
		[Display(Name = "Mobile phone number")] // "Номер мобильного телефона"
		public string PhoneNumber { get; set; }

		[BindProperty]
		[Required(ErrorMessage = Localizer.DataAnnotations.RequiredErrorMessage)]
		[DataType(DataType.Password)]
		[Display(Name = "Password")] // "Пароль"
		public string Password { get; set; }

		[BindProperty]
		[Display(Name = "Remember me")] // "Запомнить меня"
		public bool RememberMe { get; set; }


		// Wizard properties:

		public bool DisplayPhoneNumberSection { get; set; }


		public LoginPageModel(IIdentityRepository identityRepository, 
			SignInManager<SystemUser> signInManager,
			IStringLocalizer<IdentityResource> identityLocalizer,
			ILogger<LoginPageModel> logger)
			: base(logger, identityLocalizer, signInManager)
		{
			this._identityRepository = identityRepository;
		}


		public IActionResult OnGet()
		{
			if (User.Identity.IsAuthenticated)
			{
				return RedirectToPage("/Index");
			}

			DisplayPhoneNumberSection = true;

			return Page();
		}


		public async Task<IActionResult> OnPostCheckPhoneNumberAsync()
		{
			DisplayPhoneNumberSection = true;

			ModelState[nameof(this.Password)].Errors.Clear();
			ModelState[nameof(this.Password)].ValidationState = ModelValidationState.Skipped;

			if (ModelState.IsValid)
			{
				try
				{
					var user = await _identityRepository.FindByPhoneNumberAsync(this.PhoneNumber);
					if (user != null)
					{
						DisplayPhoneNumberSection = false;

						if (user.IsPasswordChangeRequired || string.IsNullOrEmpty(user.PasswordHash))
						{
							return RedirectToPage("/Account/ChangePassword", new
							{
								PhoneNumber = user.NormalizedPhoneNumber
							});
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
			DisplayPhoneNumberSection = false;

			if (ModelState.IsValid)
			{
				try
				{
					var user = await _identityRepository.FindByPhoneNumberAsync(this.PhoneNumber);
					if (user != null)
					{
						var actionResult = await this.SignInAsync(user.UserName, this.Password, this.RememberMe);
						if (actionResult != null)
							return actionResult;
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
	}
}

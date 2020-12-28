using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Localization;

using MedStat.Core.Identity;
using MedStat.Core.Interfaces;
using MedStat.Core.Resources;
using MedStat.WebAdmin.Classes.SharedResources;

namespace MedStat.WebAdmin.Pages.Account
{
	public class LoginPageModel : PageModel
	{
		private readonly ILogger<LoginPageModel> _logger;
		private readonly UserManager<SystemUser> _userManager;
		private readonly SignInManager<SystemUser> _signInManager;
		private readonly IIdentityRepository _identityRepository;
		private readonly IStringLocalizer<IdentityResource> _identityLocalizer;


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
			UserManager<SystemUser> userManager,
			SignInManager<SystemUser> signInManager,
			IStringLocalizer<IdentityResource> identityLocalizer,
			ILogger<LoginPageModel> logger)
		{
			this._identityRepository = identityRepository;
			this._userManager = userManager;
			this._signInManager = signInManager;
			this._identityLocalizer = identityLocalizer;
			this._logger = logger;
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

						if (user.IsPasswordChangeRequired 
						    || false == await _userManager.HasPasswordAsync(user))
						{
							return RedirectToPage("/Account/ChangePassword", new
							{
								PhoneNumber = user.NormalizedPhoneNumber
							});
						}
					}
					else
					{
						ViewData["error_message"] = _identityLocalizer["Invalid phone number"].Value;
					}
				}
				catch (Exception ex)
				{
					ViewData["error_message"]
						= string.Format(_identityLocalizer["Error has occurred: {0}"].Value, ex.Message);
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
						var result = await _signInManager.PasswordSignInAsync(user.UserName, this.Password, 
							this.RememberMe, true);

						if (result.Succeeded)
						{
							return RedirectToPage("/Index");
						}
						else if (result.IsLockedOut)
						{
							// Account is locked until: user.LockoutEnd?.DateTime.ToLocalTime().ToShortTimeString());
							ViewData["error_message"] = _identityLocalizer["Account is locked. Try again later."].Value;
						}
						else
						{
							ViewData["error_message"] = _identityLocalizer["Invalid password"].Value;
						}
					}
					else
					{
						ViewData["error_message"] = _identityLocalizer["Invalid phone number"].Value;
					}
				}
				catch (Exception ex)
				{
					ViewData["error_message"]
						= string.Format(_identityLocalizer["Error has occurred: {0}"].Value, ex.Message);
				}
			}

			return Page();
		}
	}
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using MedStat.Core.Identity;
using MedStat.Core.Interfaces;
using MedStat.Core.Resources;
using MedStat.WebAdmin.Classes.SharedResources;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;

namespace MedStat.WebAdmin.Pages.Account
{
	public class LoginPageModel : PageModel
	{
		private readonly ILogger<LoginPageModel> _logger;
		private readonly UserManager<SystemUser> _userManager;
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


		public LoginPageModel(IIdentityRepository identityRepository, 
			UserManager<SystemUser> userManager,
			IStringLocalizer<IdentityResource> identityLocalizer,
			ILogger<LoginPageModel> logger)
		{
			this._identityRepository = identityRepository;
			this._userManager = userManager;
			this._identityLocalizer = identityLocalizer;
			this._logger = logger;
		}


		public IActionResult OnGet()
		{
			if (User.Identity.IsAuthenticated)
			{
				return RedirectToPage("/Index");
			}

			return Page();
		}

		//[ValidateAntiForgeryToken]
		public async Task<IActionResult> OnPostAsync()
		{
			if (ModelState.IsValid)
			{
				try
				{
					var user = await _identityRepository.FindByPhoneNumberAsync(this.PhoneNumber);
					if (user != null 
					    && await _userManager.CheckPasswordAsync(user, this.Password))
					{
						var identity = new ClaimsIdentity(IdentityConstants.ApplicationScheme);
						{
							identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
							identity.AddClaim(new Claim(ClaimTypes.Name, user.UserName));
						}

						await HttpContext.SignInAsync(IdentityConstants.ApplicationScheme,
							new ClaimsPrincipal(identity), 
							new AuthenticationProperties{ IsPersistent = this.RememberMe});

						return RedirectToPage("/Index");
					}
					else
					{
						ViewData["error_message"] = _identityLocalizer["Invalid phone or password"].Value;
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

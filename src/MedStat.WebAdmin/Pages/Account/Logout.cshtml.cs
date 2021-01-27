using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Localization;

using MedStat.Core.Identity;
using MedStat.WebAdmin.Classes.SharedResources;

namespace MedStat.WebAdmin.Pages.Account
{
	public class LogoutPageModel : PageModel
	{
		private readonly SignInManager<SystemUser> _signInManager;
		private readonly IStringLocalizer<IdentityResource> _identityLocalizer;
		private readonly ILogger<LogoutPageModel> _logger;


		public LogoutPageModel(SignInManager<SystemUser> signInManager,
			IStringLocalizer<IdentityResource> identityLocalizer, ILogger<LogoutPageModel> logger)
		{
			_signInManager = signInManager;
			_identityLocalizer = identityLocalizer;
			_logger = logger;
		}


		public void OnGet()
		{
		}

		public async Task<IActionResult> OnPost(string returnUrl = null)
		{
			try
			{
				var userName = this.User?.Identity?.Name;

				await _signInManager.SignOutAsync();

				_logger.LogInformation("User {UserName} logged out.", userName);

				if (returnUrl != null)
				{
					return LocalRedirect(returnUrl);
				}
				else
				{
					return RedirectToPage();
				}
			}
			catch (Exception ex)
			{
				ViewData["error_message"] = _identityLocalizer.GetFormattedValue_ErrorHasOccurred(ex.Message);
			}

			return Page();
		}
	}
}

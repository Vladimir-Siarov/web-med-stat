using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

using MedStat.Core.Identity;
using MedStat.Core.Resources;
using MedStat.WebAdmin.Classes.SharedResources;
using Microsoft.Net.Http.Headers;

namespace MedStat.WebAdmin.Models
{
	public class ChangePhoneNumberModel
	{
		[BindProperty]
		[Required(ErrorMessage = Localizer.DataAnnotations.RequiredErrorMessage)]
		[Phone(ErrorMessage = Localizer.DataAnnotations.InvalidPhoneNumber)]
		[StringLength(20)]
		[Display(Name = "Mobile phone number")] // "Номер мобильного телефона"
		public string PhoneNumber { get; set; }

		[BindProperty]
		[Required(ErrorMessage = Localizer.DataAnnotations.RequiredErrorMessage)]
		[Phone(ErrorMessage = Localizer.DataAnnotations.InvalidPhoneNumber)]
		[StringLength(20)]
		[Display(Name = "New phone number")] // "Новый номер телефона"
		public string NewPhoneNumber { get; set; }
	}


	// Page models:

	public abstract class SignInBasePageModel : PageModel
	{
		protected ILogger Logger { get; }

		protected SignInManager<SystemUser> SignInManager { get; }

		protected IStringLocalizer<IdentityResource> IdentityLocalizer { get; }


		protected SignInBasePageModel(ILogger logger,
			IStringLocalizer<IdentityResource> identityLocalizer,
			SignInManager<SystemUser> signInManager)
		{
			this.SignInManager = signInManager;
			this.Logger = logger;
			this.IdentityLocalizer = identityLocalizer;
		}


		public async Task<IActionResult> SignInAsync(string userName, string password, bool isPersistent)
		{
			var result = await this.SignInManager.PasswordSignInAsync(userName, password,
				isPersistent, true);

			if (result.Succeeded)
			{
				this.Logger.LogInformation("User {UserName} was successfully logged in to system. {UserAgent}. {IpAddress}", 
					userName, 
					Request.Headers[HeaderNames.UserAgent].FirstOrDefault(),
					Request.HttpContext.Connection.RemoteIpAddress);

				return RedirectToPage("/Index");
			}
			else if (result.IsLockedOut)
			{
				// Account is locked until: user.LockoutEnd?.DateTime.ToLocalTime().ToShortTimeString());
				ViewData["error_message"] = this.IdentityLocalizer["Account is locked. Try again later."].Value;
			}
			else
			{
				ViewData["error_message"] = this.IdentityLocalizer["Invalid password"].Value;
			}

			return null;
		}
	}
}

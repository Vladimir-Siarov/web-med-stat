using System;
using System.Threading.Tasks;
using MedStat.Core.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Localization;

using MedStat.Core.Info.Company;
using MedStat.Core.Interfaces;
using MedStat.WebAdmin.Classes.SharedResources;
using MedStat.WebAdmin.Models;
using Microsoft.AspNetCore.Authorization;

namespace MedStat.WebAdmin.Pages.Companies.Users
{
	[Authorize(Policy = AccessPolicies.CompanyUserManageRights)]
	public class CompanyUserDeletePageModel : CompanyUserBasePageModel
	{
		[BindProperty]
		public string CmpUserName { get; set; }


		public CompanyUserDeletePageModel(ICompanyRepository cmpRepository,
			IStringLocalizer<CompanyResource> cmpLocalizer)
			: base(cmpRepository, cmpLocalizer)
		{
		}


		public async Task<IActionResult> OnGet()
		{
			var cmpUserInfo = await this.CmpRepository.GetCompanyUserInfoAsync(this.CmpUserId);
			if (cmpUserInfo == null)
				return NotFound();

			this.CmpUserName = $"{cmpUserInfo.User.Login.Surname} {cmpUserInfo.User.Login.FirstName}";

			return Page();
		}

		public async Task<IActionResult> OnPostAsync()
		{
			if (ModelState.IsValid)
			{
				try
				{
					await this.CmpRepository.DeleteCompanyUserAsync(this.CmpUserId);

					this.EntityWasDeleted = true;

					ViewData["success_message"] = this.CmpLocalizer["User was deleted successfully"];
				}
				catch (Exception ex)
				{
					ViewData["error_message"] = this.CmpLocalizer.GetFormattedValue_ErrorHasOccurred(ex.Message);
				}
			}

			return Page();
		}
  }
}

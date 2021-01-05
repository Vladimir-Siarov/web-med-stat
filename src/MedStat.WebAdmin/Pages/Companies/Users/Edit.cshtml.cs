using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Localization;

using MedStat.Core.Info.Company;
using MedStat.Core.Interfaces;
using MedStat.WebAdmin.Classes.SharedResources;
using MedStat.WebAdmin.Models;

namespace MedStat.WebAdmin.Pages.Companies.Users
{
	public class CompanyUserEditPageModel : CompanyUserBasePageModel
	{
		[BindProperty]
		public CompanyUserInfo CmpUserData { get; set; }
		

		public CompanyUserEditPageModel(ICompanyRepository cmpRepository,
			IStringLocalizer<CompanyResource> cmpLocalizer)
			: base(cmpRepository, cmpLocalizer)
		{
		}


		public async Task<IActionResult> OnGet(bool? isCreated)
		{
			this.CmpUserData = await this.CmpRepository.GetCompanyUserAsync(this.CmpUserId);
			if (this.CmpUserData == null)
				return NotFound();

			//this.CompanyName = company.Name;

			if (isCreated == true && this.CmpUserData != null)
			{
				this.EntityWasCreated = true;

				ViewData["success_message"] = string.Format(
					this.CmpLocalizer["User __CmpUserName__ was created successfully"].Value,
					this.CmpUserData.User?.Login?.FirstName,
					this.CmpUserData.User?.Login?.Surname);
			}

			return Page();
		}

		public async Task<IActionResult> OnPostAsync()
		{
			if (ModelState.IsValid)
			{
				try
				{
					await this.CmpRepository.UpdateCompanyUserAsync(this.CmpUserId,
						this.CmpUserData.User.Description,
						this.CmpUserData.User.Login,
						this.CmpUserData.CanManageCompanyAccess,
						this.CmpUserData.CanManageCompanyStaff);

					this.EntityWasUpdated = true;

					ViewData["success_message"] = this.CmpLocalizer["User data were updated"];
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

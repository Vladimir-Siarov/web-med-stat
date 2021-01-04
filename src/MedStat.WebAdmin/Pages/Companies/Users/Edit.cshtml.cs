using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Localization;
using System;
using System.Threading.Tasks;

using MedStat.Core.Info.Company;
using MedStat.Core.Interfaces;
using MedStat.WebAdmin.Classes.SharedResources;
using MedStat.WebAdmin.Models;

namespace MedStat.WebAdmin.Pages.Companies.Users
{
	public class CompanyUserEditPageModel : CompanyBasePageModel
	{
		public override EnCompanySection Section => EnCompanySection.Users;

		[BindProperty]
		public CompanyUserInfo CmpUserData { get; set; }


		[BindProperty(SupportsGet = true)]
		public int CmpUserId { get; set; }


		public CompanyUserEditPageModel(ILogger<CompanyCreatePageModel> logger,
			ICompanyRepository cmpRepository,
			IStringLocalizer<CompanyResource> cmpLocalizer)
			: base(logger, cmpRepository, cmpLocalizer)
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
					//int cmpUserId = await this.CmpRepository.CreateCompanyUserAsync(this.CompanyId,
					//	this.CmpUserData.User.Description,
					//	this.CmpUserData.User.Login,
					//	this.CmpUserData.CanManageCompanyAccess,
					//	this.CmpUserData.CanManageCompanyStaff);
					//
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

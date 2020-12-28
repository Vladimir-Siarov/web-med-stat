﻿using Microsoft.AspNetCore.Mvc;
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
	public class CompanyUserCreatePageModel : CompanyBasePageModel
	{
		public override EnCompanySection Section => EnCompanySection.Users;

		[BindProperty]
		public CompanyUserInfo CmpUserData { get; set; }


		public CompanyUserCreatePageModel(ILogger<CompanyCreatePageModel> logger,
			ICompanyRepository cmpRepository,
			IStringLocalizer<CompanyResource> cmpLocalizer)
			: base(logger, cmpRepository, cmpLocalizer)
		{
		}


		public void OnGet()
		{
		}

		public async Task<IActionResult> OnPostAsync()
		{
			if (ModelState.IsValid)
			{
				try
				{
//					int cmpId = await this.CmpRepository.CreateCompanyUserAsync(this.CmpUserData.User,
//						this.CmpUserData.CanManageCompanyAccess, 
//						this.CmpUserData.CanManageCompanyStaff);
//
//					return RedirectToPage("./Main", new { CompanyId = cmpId, isCreated = true });
				}
				catch (Exception ex)
				{
					ViewData["error_message"] 
						= string.Format(this.CmpLocalizer["Error has occurred: {0}"].Value, ex.Message);
				}
			}

			return Page();
		}
  }
}